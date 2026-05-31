using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Converters;
using Services.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Workers
{
    public class WeatherWorker : BackgroundService, IServiceWorker
    {
        private readonly ILogger _logger;
        private readonly string _weatherApiKey;

        public WeatherApi LatestWeather { get; private set; }

        public event EventHandler<WeatherApi> LatestWeatherUpdated;
        public event EventHandler<EventArgs> ProcessingStarted;
        public event EventHandler<EventArgs> ProcessingFinished;

        #region Ctor
        public WeatherWorker(ILogger logger, string weatherApiKey)
        {
            if (string.IsNullOrEmpty(weatherApiKey))
            {
                throw new ArgumentNullException(nameof(weatherApiKey), "WeatherApiComApiKey is not set");
            }

            _logger = logger;
            _weatherApiKey = weatherApiKey;
        }
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.Process();
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        public async Task Process()
        {
            _logger.LogTrace("WeatherWorker is running.");
            this.ProcessingStarted?.Invoke(this, EventArgs.Empty);

            Stopwatch s = Stopwatch.StartNew();

            this.LatestWeather = await this.GetData();
            this.LatestWeatherUpdated?.Invoke(this, this.LatestWeather);

            s.Stop();

            this.ProcessingFinished?.Invoke(this, EventArgs.Empty);
            _logger.LogTrace("WeatherWorker completed a cycle in {ElapsedMilliseconds} ms.", s.ElapsedMilliseconds);
        }

        private async Task<WeatherApi> GetData()
        {
            string json = null;

            using (HttpClient client = new())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:101.0) Gecko/20100101 Firefox/101.0");

                try
                {
                    HttpResponseMessage res = await client.GetAsync($"https://api.weatherapi.com/v1/forecast.json?key={_weatherApiKey}&q=Heinsberg&days=3&aqi=yes");

                    if (!res.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Response is not successful: {res.StatusCode}");
                    }

                    using (StreamReader r = new(await res.Content.ReadAsStreamAsync()))
                    {
                        json = await r.ReadToEndAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new HttpRequestException("Error: ", ex);
                }
            }

            if (string.IsNullOrEmpty(json))
            {
                throw new InvalidDataException("Response is empty");
            }

            if (!IsValidJson(json))
            {
                throw new InvalidDataException("Response is faulty");
            }

            try
            {
                JsonSerializerOptions options = new()
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                    Converters = 
                    { 
                        new WeatherApiDateTimeConverter(),
                        new IntToBoolConverter()
                    }
                };

                return JsonSerializer.Deserialize<WeatherApi>(json, options);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON Deserialization failed. Path: {Path}, Line: {LineNumber}, Position: {BytePositionInLine}", 
                    ex.Path, ex.LineNumber, ex.BytePositionInLine);
                throw new InvalidDataException($"Failed to deserialize JSON: {ex.Message} at path '{ex.Path}'", ex);
            }
        }

        private static bool IsValidJson(string json)
        {
            try
            {
                JsonDocument.Parse(json);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
