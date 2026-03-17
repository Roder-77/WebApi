using System.Text.Encodings.Web;
using System.Text.Json;

namespace Models.Infrastructures
{
    public static class GlobalSettings
    {
        public static JsonSerializerOptions JsonOptions => new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }
}