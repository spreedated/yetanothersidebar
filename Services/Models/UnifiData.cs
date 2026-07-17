using System;

namespace Services.Models
{
    public record UnifiData
    {
        public int ClientsConnected { get; init; }
        public float DownloadBps { get; init; }
        public float DownloadKbps
        {
            get
            {
                return (this.DownloadBps * 8) / 1000;
            }
        }
        public float DownloadMbps
        {
            get
            {
                return (this.DownloadBps * 8) / 1_000_000f;
            }
        }

        public float UploadBps { get; init; }
        public float UploadKbps
        {
            get
            {
                return (this.UploadBps * 8) / 1000;
            }
        }
        public float UploadMbps
        {
            get
            {
                return (this.UploadBps * 8) / 1_000_000f;
            }
        }

        public string Uptime { get; init; }
        public float MemoryUsage { get; init; }
        public float CpuUsage { get; init; }

        public DateTime LastUpdated { get; init; }
        public bool Error { get; init; }
    }
}
