using Models.Infrastructures;
using System.Text.Json;

namespace Services.Extensions
{
    public static class JsonExtension
    {
        public static string ToJson(this object? obj)
        {
            if (obj is null)
                return string.Empty;

            return JsonSerializer.Serialize(obj, GlobalSettings.JsonOptions);
        }

        public static T? FromJson<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, GlobalSettings.JsonOptions);
        }
    }
}
