using System;

namespace Services.Models
{
    public sealed record UnifiData
    {
        public int ClientsConnected { get; init; }
        public string Uptime { get; init; }
        public float MemoryUsage { get; init; }
        public float CpuUsage { get; init; }

        public DateTime LastUpdated { get; init; }
        public bool Error { get; init; }
    }
}
