using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Converters
{
    public class NumberToStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int intValue))
                {
                    return intValue.ToString();
                }
                if (reader.TryGetInt64(out long longValue))
                {
                    return longValue.ToString();
                }
                if (reader.TryGetDouble(out double doubleValue))
                {
                    return doubleValue.ToString();
                }
            }

            if (reader.TokenType == JsonTokenType.True)
            {
                return "true";
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return "false";
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
