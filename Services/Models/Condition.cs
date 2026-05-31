using System.Text.Json.Serialization;

namespace Services.Models
{
    public record Condition
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonIgnore()]
        public string IconAdress
        {
            get
            {
                return $"https:{this.Icon}";
            }
        }
    }
}
