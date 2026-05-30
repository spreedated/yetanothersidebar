#pragma warning disable S1075 // Suppress "URIs should not be hardcoded" warning since this is a well-known API endpoint that is unlikely to change

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
    public class GodotWorker : BackgroundService
    {
        private readonly ILogger<GodotWorker> _logger;

        public Version LatestVersion { get; private set; }

        public event EventHandler<Version> LatestVersionUpdated;

        public GodotWorker(ILogger<GodotWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("GodotWorker is running.");
                Stopwatch s = Stopwatch.StartNew();

                HtmlDocument doc = await this.DownloadSourcecode();
                this.LatestVersion = ParseSourcecode(doc);
                this.LatestVersionUpdated?.Invoke(this, this.LatestVersion);

                s.Stop();
                _logger.LogTrace("GodotWorker completed a cycle in {ElapsedMilliseconds} ms.", s.ElapsedMilliseconds);
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
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:101.0) Gecko/20100101 Firefox/101.0");

                    HttpResponseMessage response = await client.GetAsync("https://godotengine.org/download/windows/", HttpCompletionOption.ResponseHeadersRead);

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
                _logger.LogError(ex, "Error occurred while downloading Godot source code.");
                return null;
            }
        }

        private static Version ParseSourcecode(HtmlDocument doc)
        {
            if (doc == null)
            {
                return default;
            }

            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'download-hint')]");

            if (node == null)
            {
                return default;
            }

            if (Version.TryParse(node.InnerHtml, out Version v))
            {
                return v;
            }

            return default;
        }
    }
}
