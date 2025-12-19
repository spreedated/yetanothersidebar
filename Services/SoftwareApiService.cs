#pragma warning disable S112

using Services.Models.Responses;
using System;
using System.Text.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using HtmlAgilityPack;
using System.Linq;

namespace Services
{
    public sealed class SoftwareApiService : Service<SoftwareResponse>
    {
        public const string GODOT_URL = "https://godotengine.org/download/windows/";

        public SoftwareApiService()
        {
            base.CreateTimer(new TimeSpan(24, 0, 0));
        }

        internal override void Processor(object sender, ElapsedEventArgs e)
        {
            this.ProcessIsRunning = true;
            base.RaiseProcessStarted();

            try
            {
                this.GetData().Wait();
            }
            catch (Exception ex)
            {
                base.RaiseError(ex);
            }

            this.ProcessIsRunning = false;
            base.RaiseValuesUpdated();
        }

        private async Task GetData()
        {
            SoftwareResponse res = new();

            Task<Version> godot = Task.Run<Version>(() => ReadGodotScraper());

            await Task.WhenAll(godot);

            res.Godot = godot.Result;

            res.Date = DateTime.UtcNow;

            base.Response = res;
        }

        private static async Task<Version> ReadGodotScraper()
        {
            string html = null;

            using (HttpClient hc = new())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:101.0) Gecko/20100101 Firefox/101.0");

                HttpResponseMessage response = await hc.GetAsync(GODOT_URL);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return default;
                }

                html = await response.Content.ReadAsStringAsync();
            }

            HtmlDocument doc = new();
            doc.LoadHtml(html);

            HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'download-hint')]");

            if (node == null)
            {
                return default;
            }
            
            if (!Version.TryParse(node.InnerText, out Version v))
            {
                return default;
            }

            return v;
        }
    }
}
