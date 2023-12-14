using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Models;
using Models.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebApi.Filters
{
    public class SwaggerHeaderParameter : IOperationFilter
    {
        private IEnumerable<SwaggerHeader> _headers;

        public SwaggerHeaderParameter(IEnumerable<SwaggerHeader>? headers = null)
        {
            _headers = headers ?? Enumerable.Empty<SwaggerHeader>();
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (HasActionAttribute<AllowAnonymousAttribute>(context) || HasActionAttribute<IgnoreValidPermission>(context))
                return;

            operation.Parameters ??= new List<OpenApiParameter>();

            foreach (var header in _headers)
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = header.Name,
                    Description = header.Description,
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema() { Type = "String" },
                    Required = header.Required,
                    Example = new OpenApiString(header.Example)
                });
        }

        private bool HasActionAttribute<T>(OperationFilterContext context) where T : Attribute
            => context.MethodInfo?.GetCustomAttribute<T>(false) is not null;
    }
}
