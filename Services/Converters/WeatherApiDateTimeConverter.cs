using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Converters
{
    public class WeatherApiDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] Formats =
        [
            "yyyy-MM-dd HH:mm",     // "2026-05-31 03:35"
            "yyyy-MM-dd",            // "2026-05-31"
            "yyyy-MM-dd HH:mm:ss",   // Just in case
        ];

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString();

            if (string.IsNullOrEmpty(dateString))
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParseExact(dateString, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            // Fallback to standard parsing
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            throw new JsonException($"Unable to parse '{dateString}' as DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
        }
    }
}
