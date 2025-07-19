using System.Text.Json.Serialization;

namespace Services.Models.Responses
{
    public sealed record IssResponse
    {
        [JsonPropertyName("NODE3000005")]
        public float UrineTankQuantity { get; set; }

        [JsonPropertyName("NODE3000008")]
        public float WasteWaterTankQuantity { get; set; }

        [JsonPropertyName("NODE3000009")]
        public float CleanWaterTankQuantity { get; set; }
    }
}
