using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Services.Models
{
    public record Forecastday
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("date_epoch")]
        public int DateEpoch { get; set; }

        [JsonPropertyName("day")]
        public Day Day { get; set; }

        [JsonPropertyName("astro")]
        public Astro Astro { get; set; }

        [JsonPropertyName("hour")]
        public List<Hour> Hour { get; set; }
    }
}
