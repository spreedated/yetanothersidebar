using System;
using System.Text.Json.Serialization;

namespace ServiceLGAgentBattery.Models
{
    internal class Configuration
    {
        [JsonPropertyName("startservicetime")]
        public TimeSpan StartServiceTime { get; internal set; } = new TimeSpan(0, 0, 0);

        [JsonPropertyName("endservicetime")]
        public TimeSpan EndServiceTime { get; internal set; } = new TimeSpan(23, 59, 59);

        [JsonPropertyName("interval")]
        public TimeSpan TimeInterval { get; internal set; } = new TimeSpan(0, 0, 30);

        [JsonPropertyName("runday")]
        public int Runday { get; set; } = 32;
    }
}