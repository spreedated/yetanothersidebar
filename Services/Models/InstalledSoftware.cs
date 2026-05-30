using System;
using System.Text.Json.Serialization;

namespace Services.Models
{
    public record InstalledSoftware
    {
        [JsonPropertyName("godot")]
        public Version Godot { get; set; }

        [JsonPropertyName("blender")]
        public Version Blender { get; set; }

        [JsonPropertyName("bluejay")]
        public Version Bluejay { get; set; }

        [JsonPropertyName("bffw")]
        public Version BetaflightFw { get; set; }

        [JsonPropertyName("elrs")]
        public Version ExpressLrs { get; set; }

        [JsonPropertyName("etx")]
        public Version EdgeTx { get; set; }

        [JsonPropertyName("whoopstor")]
        public Version Whoopstor { get; set; }
    }
}
