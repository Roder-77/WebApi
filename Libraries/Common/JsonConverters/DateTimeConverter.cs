using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.JsonConverters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        // Deserialization
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString() ?? string.Empty);

        // Serialization
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
