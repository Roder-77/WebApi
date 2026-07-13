using System.Text.Json;

namespace WebApi.Utils
{
    public class CamelCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value is null) return null;

            return JsonNamingPolicy.CamelCase.ConvertName(value.ToString()!);
        }
    }
}