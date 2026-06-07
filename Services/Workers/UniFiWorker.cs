#pragma warning disable S4830 // S4830: Don't use HttpClient with an insecure SSL/TLS configuration
#pragma warning disable S1075 // S1075: URIs should not be hardcoded

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using Services.Models;
using Services.RefitInterfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Workers
{
    public class UniFiWorker : BackgroundService, IServiceWorker
    {
        private readonly ILogger _logger;
        private readonly string[] _validSslThumbprints;
        private readonly LoginRequest _credentials;
        private CookieContainer _cookieContainer;

        public event EventHandler<UnifiData> LatestUnifiUpdated;
        public event EventHandler<EventArgs> ProcessingStarted;
        public event EventHandler<EventArgs> ProcessingFinished;

        public UnifiData LatestUnifiData { get; private set; }

        #region Ctor
        public UniFiWorker(ILogger logger, LoginRequest credentials, string[] validSslThumbprints)
        {
            _logger = logger;
            _credentials = credentials;
            _validSslThumbprints = validSslThumbprints;
        }
        #endregion

        private HttpClient CreateHttpClient()
        {
            HttpClientHandler handler = new()
            {
                CookieContainer = _cookieContainer,
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            handler.ServerCertificateCustomValidationCallback =
            (request, cert, chain, errors) =>
            {
                if (cert != null && !string.IsNullOrEmpty(cert.Thumbprint))
                {
                    return _validSslThumbprints.Any(tp => cert.Thumbprint.Equals(tp, StringComparison.InvariantCultureIgnoreCase));
                }

                return false;
            };

            return new(handler)
            {
                Timeout = TimeSpan.FromSeconds(10),
                DefaultRequestHeaders =
                {
                    { "Accept", "*/*" },
                    { "User-Agent", "YetAnotherMonitor/1.0" }
                },
                BaseAddress = new Uri("https://192.168.1.1/")
            };
        }

        private async Task GetLoginCookie()
        {
            _cookieContainer = new();

            using (HttpClient client = this.CreateHttpClient())
            {
                IUnifiApi rr = RestService.For<IUnifiApi>(client);

                ApiResponse<string> loginResponse = await rr.LoginAsync(_credentials);

                if (loginResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }
            }
        }

        public async Task Process()
        {
            if (_cookieContainer == null || _cookieContainer.Count <= 0)
            {
                await this.GetLoginCookie();
            }

            using (HttpClient client = this.CreateHttpClient())
            {
                IUnifiApi rr = RestService.For<IUnifiApi>(client);

                ApiResponse<string> ss = await rr.GetHealthAsync();

                if (ss.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await this.GetLoginCookie();
                    await this.Process();
                    return;
                }

                JsonDocument doc = JsonDocument.Parse(ss.Content);

                int uploadThroughput = doc.RootElement.GetProperty("data").EnumerateArray().FirstOrDefault(x => x.GetProperty("subsystem").GetString() == "www").GetProperty("tx_bytes-r").GetInt32();
                int downloadThroughput = doc.RootElement.GetProperty("data").EnumerateArray().FirstOrDefault(x => x.GetProperty("subsystem").GetString() == "www").GetProperty("rx_bytes-r").GetInt32();

                ApiResponse<string> statsResponse = await rr.GetStatsAsync();

                JsonDocument docStats = JsonDocument.Parse(statsResponse.Content);

                int connectedClients = docStats.RootElement.GetProperty("data").EnumerateArray().Count();

                ApiResponse<string> deviceResponse = await rr.GetDeviceAsync();

                JsonDocument docDevice = JsonDocument.Parse(deviceResponse.Content);

                JsonElement systemStats = docDevice.RootElement.GetProperty("data").EnumerateArray().First().GetProperty("system-stats");

                float cpuUsage = float.Parse(systemStats.GetProperty("cpu").GetString(), CultureInfo.InvariantCulture);
                float memUsage = float.Parse(systemStats.GetProperty("mem").GetString(), CultureInfo.InvariantCulture);
                long uptime = long.Parse(systemStats.GetProperty("uptime").GetString(), CultureInfo.InvariantCulture);

                TimeSpan uptimeSpan = TimeSpan.FromSeconds(uptime);

                string uptimeString = $"{(int)uptimeSpan.TotalDays}d {uptimeSpan.Hours:D2}h {uptimeSpan.Minutes:D2}m {uptimeSpan.Seconds:D2}s";

                this.LatestUnifiData = new()
                {
                    CpuUsage = cpuUsage,
                    MemoryUsage = memUsage,
                    Uptime = uptimeString,
                    ClientsConnected = connectedClients,
                    DownloadBps = downloadThroughput,
                    UploadBps = uploadThroughput,
                    LastUpdated = DateTime.Now
                };
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    this.ProcessingStarted?.Invoke(this, EventArgs.Empty);

                    await this.Process();

                    this.ProcessingFinished?.Invoke(this, EventArgs.Empty);
                    this.LatestUnifiUpdated?.Invoke(this, this.LatestUnifiData);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Process error");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
