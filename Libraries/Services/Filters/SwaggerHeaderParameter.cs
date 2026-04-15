using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Models.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Services.Filters
{
    public class SwaggerHeaderParameter : IOperationFilter
    {
        private IEnumerable<OpenApiParameter> _headers;

        public SwaggerHeaderParameter(IEnumerable<OpenApiParameter>? headers = null)
        {
            _headers = headers ?? [];
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (HasActionAttribute<AllowAnonymousAttribute>(context) || HasActionAttribute<IgnoreValidPermission>(context))
                return;

            operation.Parameters ??= [];

            foreach (var header in _headers)
            {
                operation.Parameters.Add(header);
            }
        }

        private bool HasActionAttribute<T>(OperationFilterContext context) where T : Attribute
            => context.MethodInfo?.GetCustomAttribute<T>(false) is not null;
    }
}
