using System;

namespace Services.Models
{
    public record FpvSoftwareVersions
    {
        public Version BetaflightFw { get; set; }
        public Version ExpressLRS { get; set; }
        public Version EdgeTX { get; set; }
        public Version BlueJay { get; set; }
        public Version Whoopstor3 { get; set; }
        public DateTime Date { get; set; }
    }
}
