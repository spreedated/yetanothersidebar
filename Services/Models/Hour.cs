using System;
using System.Text.Json.Serialization;

namespace Services.Models
{
    public record Hour
    {
        [JsonPropertyName("time_epoch")]
        public int TimeEpoch { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("temp_f")]
        public double TempF { get; set; }

        [JsonPropertyName("is_day")]
        public int IsDay { get; set; }

        [JsonPropertyName("condition")]
        public Condition Condition { get; set; }

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("wind_degree")]
        public int WindDegree { get; set; }

        [JsonPropertyName("wind_dir")]
        public string WindDir { get; set; }

        [JsonPropertyName("pressure_mb")]
        public double PressureMb { get; set; }

        [JsonPropertyName("precip_mm")]
        public double PrecipMm { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("cloud")]
        public int Cloud { get; set; }

        [JsonPropertyName("feelslike_c")]
        public double FeelslikeC { get; set; }

        [JsonPropertyName("windchill_c")]
        public double WindchillC { get; set; }

        [JsonPropertyName("heatindex_c")]
        public double HeatindexC { get; set; }

        [JsonPropertyName("dewpoint_c")]
        public double DewpointC { get; set; }

        [JsonPropertyName("will_it_rain")]
        public int WillItRain { get; set; }

        [JsonPropertyName("chance_of_rain")]
        public int ChanceOfRain { get; set; }

        [JsonPropertyName("will_it_snow")]
        public int WillItSnow { get; set; }

        [JsonPropertyName("chance_of_snow")]
        public int ChanceOfSnow { get; set; }

        [JsonPropertyName("vis_km")]
        public double VisKm { get; set; }

        [JsonPropertyName("gust_kph")]
        public double GustKph { get; set; }

        [JsonPropertyName("uv")]
        public double Uv { get; set; }

        [JsonPropertyName("air_quality")]
        public AirQuality AirQuality { get; set; }
    }
}
