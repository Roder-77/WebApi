using System.Text.RegularExpressions;

namespace WebApi.Utils
{
    public class CamelCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value is null) return null;

            var arr = Regex.Split(value.ToString()!, "(?<=[a-z])(?=[A-Z])");
            arr[0] = arr[0].ToLower();

            return string.Join("", arr);
        }
    }
}
