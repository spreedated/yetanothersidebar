#pragma warning disable S1075

using Services.Models.Responses;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public sealed class NzbService : Service<NzbResponse>
    {
        public NzbService()
        {
            base.CreateTimer(new TimeSpan(20, 0, 0));
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
            base.Response = new()
            {
                IsDrunkenSlugOpen = await this.IsOpen("https://drunkenslug.com/register"),
                IsNinjaCentralOpen = await this.IsOpen("https://ninjacentral.co.za/register"),
                Date = DateTime.Now
            };
        }

        private async Task<bool> IsOpen(string url)
        {
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return false;
                }

                string sourcecode = await response.Content.ReadAsStringAsync();

                if (sourcecode.Contains("Registrations are currently disabled.", StringComparison.InvariantCultureIgnoreCase) || sourcecode.ToLower().Contains("The Bar is closed", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
