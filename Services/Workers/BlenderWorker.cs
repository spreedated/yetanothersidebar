using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using neXn.Lib.Strings;
using neXn.Lib;

namespace Services.Workers
{
    public class BlenderWorker : ServiceWorker
    {
        private readonly ILogger<BlenderWorker> _logger;

        public Version LatestVersion { get; private set; }

        public event EventHandler<Version> LatestVersionUpdated;

        public BlenderWorker(ILogger<BlenderWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!base.IsEnabled)
                {
                    _logger?.LogTrace("BlenderWorker is disabled. Skipping processing.");
                    return;
                }

                try
                {
                    base.RaiseProcessedStarted();

                    await this.Process();

                    base.RaiseProcessedFinished();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Process error");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    continue;
                }

                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }

        public override async Task Process()
        {
            HtmlDocument doc = await this.DownloadSourcecode();
            this.LatestVersion = this.ParseSourcecode(doc);
            this.LatestVersionUpdated?.Invoke(this, this.LatestVersion);
        }

        private async Task<HtmlDocument> DownloadSourcecode()
        {
            try
            {
                HtmlDocument doc = new();

                using (HttpClient client = new())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    HttpResponseMessage response = await client.GetAsync("https://www.blender.org/download/", HttpCompletionOption.ResponseHeadersRead);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        doc.Load(stream);
                    }
                }

                return doc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while downloading Blender source code.");
                return null;
            }
        }

        private Version ParseSourcecode(HtmlDocument doc)
        {
            if (doc == null)
            {
                return default;
            }

            var node = doc.DocumentNode.SelectSingleNode("//ul[@class='dl-build-details mb-0']/li[3]");

            if (node == null)
            {
                return default;
            }

            if (Version.TryParse(node.InnerHtml.AllowOnlyCharacters("0123456789."), out Version v))
            {
                return v;
            }

            return default;
        }
    }
}
