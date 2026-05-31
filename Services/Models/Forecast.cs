using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Services.Models
{
    public record Forecast
    {
        [JsonPropertyName("forecastday")]
        public List<Forecastday> Forecastday { get; set; }
    }
}
