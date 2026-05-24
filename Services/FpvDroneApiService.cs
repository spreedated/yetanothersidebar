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
    public sealed class FpvDroneApiService : Service<FpvDroneResponse>
    {
        public const string WHOOPSTOR3_URL = "https://viflydrone.com/pages/download-center";

        public FpvDroneApiService()
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
            FpvDroneResponse res = new();

            Task<Version> e = Task.Run<Version>(() => ReadVersionFromGithubJson("https://api.github.com/repos/ExpressLRS/ExpressLRS/releases/latest"));
            Task<Version> bfw = Task.Run<Version>(() => ReadVersionFromGithubJson("https://api.github.com/repos/betaflight/betaflight/releases/latest"));
            Task<Version> etx = Task.Run<Version>(() => ReadVersionFromGithubJson("https://api.github.com/repos/EdgeTX/EdgeTX/releases/latest"));
            Task<Version> bjay = Task.Run<Version>(() => ReadVersionFromGithubJson("https://api.github.com/repos/bird-sanctuary/bluejay/releases/latest"));
            Task<Version> whoop = Task.Run<Version>(() => ReadWhoopstor3Scraper());

            await Task.WhenAll(e, bfw, etx, bjay, whoop);

            res.ExpressLRS = e.Result;
            res.EdgeTX = etx.Result;
            res.BetaflightFw = bfw.Result;
            res.BlueJay = bjay.Result;
            res.Whoopstor3 = whoop.Result;

            res.Date = DateTime.UtcNow;

            base.Response = res;
        }

        private static async Task<Version> ReadWhoopstor3Scraper()
        {
            string html = null;

            using (HttpClient hc = new())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:101.0) Gecko/20100101 Firefox/101.0");

                HttpResponseMessage response = await hc.GetAsync(WHOOPSTOR3_URL);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return default;
                }

                html = await response.Content.ReadAsStringAsync();
            }

            HtmlDocument doc = new();
            doc.LoadHtml(html);

            HtmlNode node = doc.DocumentNode.SelectSingleNode("//a[contains(@title, 'WhoopStor') and contains(@title, 'V3')]");

            if (node == null)
            {
                return default;
            }

            string vers = node.Attributes["title"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();

            if (!Version.TryParse(vers, out Version v))
            {
                return default;
            }

            return v;
        }

        private static async Task<Version> ReadVersionFromGithubJson(string url)
        {
            string json = null;

            using (HttpClient client = new())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:101.0) Gecko/20100101 Firefox/101.0");

                HttpResponseMessage res = await client.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    return default;
                }

                using (StreamReader r = new(await res.Content.ReadAsStreamAsync()))
                {
                    json = await r.ReadToEndAsync();
                }
            }

            JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("tag_name", out JsonElement tag))
            {
                string v = tag.GetString().Replace("v","");
                if (Version.TryParse(v, out Version ver))
                {
                    return ver;
                }
            }

            return default;
        }
    }
}
