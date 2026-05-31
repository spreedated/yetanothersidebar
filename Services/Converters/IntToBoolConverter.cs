using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Converters
{
    public class IntToBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() != 0;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                if (bool.TryParse(value, out bool result))
                {
                    return result;
                }
                if (int.TryParse(value, out int intValue))
                {
                    return intValue != 0;
                }
            }

            throw new JsonException($"Unable to convert '{reader.GetString()}' to Boolean.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
