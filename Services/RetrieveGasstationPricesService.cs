
using HtmlAgilityPack;
using Serilog;
using Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public class RetrieveGasstationPricesService : Service<GasstationPrices>
    {
        public RetrieveGasstationPricesService()
        {
            base.CreateTimer(new TimeSpan(0, 30, 0));
        }

        internal override void Processor(object sender, ElapsedEventArgs e)
        {
            this.ProcessIsRunning = true;
            base.RaiseProcessStarted();

            bool avail = false;

            using (Ping p = new())
            {
                avail = p.Send("tankenbilliger.de").Status == IPStatus.Success;
            }

            if (!avail)
            {
                base.RaiseError(null);
                this.ProcessIsRunning = false;
                return;
            }

            if (base.Response == null)
            {
                base.Response = new();
            }

            List<Task> t =
            [
                Task.Factory.StartNew(() =>
                {
                    base.Response.ShellHeinsberg = RetrieveShell();
                }),
                Task.Factory.StartNew(() =>
                {
                    base.Response.PmExpressDremmen = RetrievePmExpress();
                }),
                Task.Factory.StartNew(() =>
                {
                    base.Response.BftOberbruch = RetrieveBft();
                })
            ];

            Task.WaitAll([.. t], new TimeSpan(0, 1, 30));

            this.ProcessIsRunning = false;
            base.RaiseValuesUpdated();
            Log.Information($"Gasprices service process ran");
        }

        private static GasstationPrices.GasPrice RetrieveShell()
        {
            return RetrieveGaspricesFromHtml(DownloadSourceCode("43433"));
        }

        private static GasstationPrices.GasPrice RetrievePmExpress()
        {
            return RetrieveGaspricesFromHtml(DownloadSourceCode("43025"));
        }

        private static GasstationPrices.GasPrice RetrieveBft()
        {
            return RetrieveGaspricesFromHtml(DownloadSourceCode("44850"));
        }

        private static string DownloadSourceCode(string id)
        {
            string sourcecode = null;

            using (HttpClient client = new())
            {
                client.Timeout = new TimeSpan(0, 0, 30);
                HttpResponseMessage resposne = client.GetAsync($"https://www.tankenbilliger.de/details.php?tankstelle={id}").Result;

                using (Stream s = resposne.Content.ReadAsStream())
                {
                    using (StreamReader r = new(s))
                    {
                        sourcecode = r.ReadToEnd();
                    }
                }
            }
            return sourcecode;
        }

        private static GasstationPrices.GasPrice RetrieveGaspricesFromHtml(string sourcecode)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(sourcecode);

            IEnumerable<HtmlNode> nodes = doc.DocumentNode.SelectNodes("//div[@class='panel-body']/table/tr").Take(3);

            string super = nodes.FirstOrDefault(x => x.InnerText.ToLower().Contains("super"))?.InnerText;
            double superprice = 0d;
            if (super != null && double.TryParse(super.Substring(super.IndexOf(' '), super.LastIndexOf(' ') - super.IndexOf(' ')).Trim(), out _))
            {
                superprice = Convert.ToDouble(super.Substring(super.IndexOf(' '), super.LastIndexOf(' ') - super.IndexOf(' ')).Trim(), CultureInfo.InvariantCulture);
            }

            string e10 = nodes.FirstOrDefault(x => x.InnerText.ToLower().Contains("e10"))?.InnerText;
            double e10price = 0d;
            if (e10 != null && double.TryParse(e10.Substring(e10.IndexOf(' '), e10.LastIndexOf(' ') - e10.IndexOf(' ')).Trim(), out _))
            {
                e10price = Convert.ToDouble(e10.Substring(e10.IndexOf(' '), e10.LastIndexOf(' ') - e10.IndexOf(' ')), CultureInfo.InvariantCulture);
            }

            string diesel = nodes.FirstOrDefault(x => x.InnerText.ToLower().Contains("diesel"))?.InnerText;
            double dieselprice = 0d;
            if (diesel != null && double.TryParse(diesel.Substring(diesel.IndexOf(' '), diesel.LastIndexOf(' ') - diesel.IndexOf(' ')).Trim(), out _))
            {
                dieselprice = Convert.ToDouble(diesel.Substring(diesel.IndexOf(' '), diesel.LastIndexOf(' ') - diesel.IndexOf(' ')).Trim(), CultureInfo.InvariantCulture);
            }

            return new()
            {
                E5 = superprice,
                E10 = e10price,
                Diesel = dieselprice,
                Date = DateTime.Now
            };
        }
    }
}
