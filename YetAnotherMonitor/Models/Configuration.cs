using System.Text.Json.Serialization;

namespace YetAnotherMonitor.Models
{
    internal class Configuration
    {
        [JsonPropertyName("windowstartuplocation")]
        public WindowLocation WindowStartupLocation { get; set; }
        [JsonPropertyName("weatherapicom-apikey")]
        public string WeatherApiComApiKey { get; set; }
    }
}
