using System.Text.Json.Serialization;

namespace Services.Models
{
    public record WeatherApi
    {
        [JsonPropertyName("location")]
        public Location WeatherLocation { get; set; }

        [JsonPropertyName("current")]
        public Current WeatherCurrent { get; set; }

        [JsonPropertyName("forecast")]
        public Forecast WeatherForecast { get; set; }
    }
}