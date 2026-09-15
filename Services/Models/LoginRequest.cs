using System.Text.Json.Serialization;

namespace Services.Models
{
    public sealed record LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("password")]
        public string Password { get; init; }
    }
}
