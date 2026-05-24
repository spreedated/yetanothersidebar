using System.Text.Json.Serialization;

namespace YamAva.Models
{
    public sealed record Configuration
    {
        [JsonPropertyName("lastWindowPosition")]
        public neXn.Ui.Models.Location LastWindowPosition { get; set; }

        [JsonPropertyName("topmost")]
        public bool TopMost { get; set; } = true;

        [JsonPropertyName("theme")]
        public bool DarkTheme { get; set; } = true;

        [JsonPropertyName("weatherapicom-apikey")]
        public string WeatherApiComApiKey { get; set; }
    }
}
