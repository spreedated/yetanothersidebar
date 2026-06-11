#pragma warning disable S1075 // Suppress "URIs should not be hardcoded" warning since the Weather API base URL is unlikely to change and is more readable as a constant

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using Services.Converters;
using Services.Models;
using Services.RefitInterfaces;
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

        public const string WEATHER_API_BASE_URL = "https://api.weatherapi.com/v1/";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            Converters = 
            { 
                new WeatherApiDateTimeConverter(),
                new IntToBoolConverter()
            }
        };

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
                try
                {
                    await this.Process();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Process error");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    continue;
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        public async Task Process()
        {
            _logger?.LogTrace("WeatherWorker is running.");
            this.ProcessingStarted?.Invoke(this, EventArgs.Empty);

            Stopwatch s = Stopwatch.StartNew();

            this.LatestWeather = await this.GetData();
            this.LatestWeatherUpdated?.Invoke(this, this.LatestWeather);

            s.Stop();

            this.ProcessingFinished?.Invoke(this, EventArgs.Empty);
            _logger?.LogTrace("WeatherWorker completed a cycle in {ElapsedMilliseconds} ms.", s.ElapsedMilliseconds);
        }

        private async Task<WeatherApi> GetData()
        {
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions)
            };
            IWeatherEndpoints restService = RestService.For<IWeatherEndpoints>(WEATHER_API_BASE_URL, refitSettings);

            try
            {
                return await restService.GetWeatherAsync(_weatherApiKey);
            }
            catch (JsonException ex)
            {
                _logger?.LogError(ex, "JSON Deserialization failed. Path: {Path}, Line: {LineNumber}, Position: {BytePositionInLine}", ex.Path, ex.LineNumber, ex.BytePositionInLine);
                throw new InvalidDataException($"Failed to deserialize JSON: {ex.Message} at path '{ex.Path}'", ex);
            }
        }
    }
}
