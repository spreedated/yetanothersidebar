#pragma warning disable S112

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Models.Responses;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public sealed class WeatherApiService : Service<WeatherApiResponse>
    {
        private readonly string weatherApiKey;

        public WeatherApiService(string apiKey)
        {
            this.weatherApiKey = apiKey;
            base.CreateTimer(new TimeSpan(0, 30, 0));
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
            if (string.IsNullOrEmpty(this.weatherApiKey))
            {
                throw new Exception("WeatherApiComApiKey is not set");
            }

            string json = null;

            using (HttpClient client = new())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:101.0) Gecko/20100101 Firefox/101.0");

                HttpResponseMessage res = await client.GetAsync($"https://api.weatherapi.com/v1/forecast.json?key={this.weatherApiKey}&q=Heinsberg&days=3&aqi=yes");

                if (!res.IsSuccessStatusCode)
                {
                    throw new Exception($"Response is not successfull: {res.StatusCode}");
                }

                using (StreamReader r = new(await res.Content.ReadAsStreamAsync()))
                {
                    json = r.ReadToEnd();
                }
            }


            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("Response is empty");
            }

            if (!IsValidJson(json))
            {
                throw new Exception("Response is faulty");
            }

            base.Response = JsonConvert.DeserializeObject<WeatherApiResponse>(json);
        }

        private static bool IsValidJson(string json)
        {
            try
            {
                JObject.Parse(json);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
