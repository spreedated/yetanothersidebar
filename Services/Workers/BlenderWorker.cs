using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Workers
{
    public class BlenderWorker : BackgroundService
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
                _logger.LogTrace("BlenderWorker is running.");
                Stopwatch s = Stopwatch.StartNew();

                HtmlDocument doc = await this.DownloadSourcecode();
                this.LatestVersion = this.ParseSourcecode(doc);
                this.LatestVersionUpdated?.Invoke(this, this.LatestVersion);

                s.Stop();
                _logger.LogTrace("BlenderWorker completed a cycle in {ElapsedMilliseconds} ms.", s.ElapsedMilliseconds);
                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
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

            if (Version.TryParse(node.InnerHtml.Replace("v", ""), out Version v))
            {
                return v;
            }

            return default;
        }
    }
}
