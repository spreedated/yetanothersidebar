#pragma warning disable S112

using Services.Models.Responses;
using System;
using System.Text.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public sealed class FpvDroneApiService : Service<FpvDroneResponse>
    {

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

            Task<Version> e = Task.Run<Version>(() => ReadVersionFromJson("https://api.github.com/repos/ExpressLRS/ExpressLRS/releases/latest"));
            Task<Version> bfw = Task.Run<Version>(() => ReadVersionFromJson("https://api.github.com/repos/betaflight/betaflight/releases/latest"));
            Task<Version> bfc = Task.Run<Version>(() => ReadVersionFromJson("https://api.github.com/repos/betaflight/betaflight-configurator/releases/latest"));
            Task<Version> etx = Task.Run<Version>(() => ReadVersionFromJson("https://api.github.com/repos/EdgeTX/EdgeTX/releases/latest"));
            Task<Version> bjay = Task.Run<Version>(() => ReadVersionFromJson("https://api.github.com/repos/bird-sanctuary/bluejay/releases/latest"));

            await Task.WhenAll(e, bfw, bfc, etx, bjay);

            res.ExpressLRS = e.Result;
            res.EdgeTX = etx.Result;
            res.BetaflightFw = bfw.Result;
            res.BetaflightConf = bfc.Result;
            res.BlueJay = bjay.Result;

            res.Date = DateTime.UtcNow;

            base.Response = res;
        }

        private static async Task<Version> ReadVersionFromJson(string url)
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
