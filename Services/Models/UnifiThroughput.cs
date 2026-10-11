using System;

namespace Services.Models
{
    public sealed record UnifiThroughput
    {
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

        public DateTime LastUpdated { get; init; }
        public bool Error { get; init; }
    }
}
