#pragma warning disable S4830 // S4830: Don't use HttpClient with an insecure SSL/TLS configuration
#pragma warning disable S1075 // S1075: URIs should not be hardcoded

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
    public class UniFiThroughputWorker : ServiceWorker
    {
        private const int _defaultIntervalSeconds = 5;

        private readonly ILogger _logger;
        private readonly string[] _validSslThumbprints;
        private readonly LoginRequest _credentials;
        private CookieContainer _cookieContainer;

        public event EventHandler<UnifiThroughput> LatestUnifiThroughputUpdated;

        public UnifiThroughput LatestUnifiThroughput { get; private set; }

        #region Ctor
        public UniFiThroughputWorker(ILogger logger, LoginRequest credentials, string[] validSslThumbprints)
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
                IUnifiEndpoints rr = RestService.For<IUnifiEndpoints>(client);

                ApiResponse<string> loginResponse = await rr.LoginAsync(_credentials);

                if (loginResponse.Error != null)
                {
                    throw new EntryPointNotFoundException("Service Unavailable");
                }

                if (loginResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }
            }
        }

        public override async Task Process()
        {
            if (_cookieContainer == null || _cookieContainer.Count <= 0)
            {
                await this.GetLoginCookie();
            }

            using (HttpClient client = this.CreateHttpClient())
            {
                IUnifiEndpoints rr = RestService.For<IUnifiEndpoints>(client);

                ApiResponse<string> ss = await rr.GetGatewayAsync("90:41:b2:7a:b1:ae");

                if (ss.Error != null)
                {
                    throw new EntryPointNotFoundException("Service Unavailable");
                }

                if (ss.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await this.GetLoginCookie();
                    await this.Process();
                    return;
                }

                JsonDocument doc = JsonDocument.Parse(ss.Content);

                int uploadThroughput = doc.RootElement.GetProperty("data").EnumerateArray().First().GetProperty("wan1").GetProperty("tx_bytes-r").GetInt32();
                int downloadThroughput = doc.RootElement.GetProperty("data").EnumerateArray().First().GetProperty("wan1").GetProperty("rx_bytes-r").GetInt32();

                this.LatestUnifiThroughput = new()
                {
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
                if (!base.IsEnabled)
                {
                    _logger?.LogTrace("UniFiWorker is disabled. Skipping processing.");
                    return;
                }

                try
                {
                    base.RaiseProcessedStarted();

                    await this.Process();

                    base.RaiseProcessedFinished();
                    this.LatestUnifiThroughputUpdated?.Invoke(this, this.LatestUnifiThroughput);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Process error");
                    await Task.Delay(TimeSpan.FromSeconds(_defaultIntervalSeconds), stoppingToken);

                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(_defaultIntervalSeconds), stoppingToken);
            }
        }
    }
}
