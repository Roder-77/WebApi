using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Models.Attributes;
using System.Text.Json.Nodes;

namespace Services.Filters
{
    public class OpenApiHeaderParameter : IOpenApiOperationTransformer
    {
        private readonly IEnumerable<OpenApiParameter> _headers;

        public OpenApiHeaderParameter(IEnumerable<OpenApiParameter>? headers = null)
        {
            _headers = headers ?? [];
        }

        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (HasAttribute<AllowAnonymousAttribute>(context) || HasAttribute<IgnoreValidPermission>(context))
                return Task.CompletedTask;

            operation.Parameters ??= [];

            foreach (var header in _headers)
            {
                operation.Parameters.Add(header);
            }

            return Task.CompletedTask;
        }

        private static bool HasAttribute<T>(OpenApiOperationTransformerContext context) where T : Attribute
            => context.Description.ActionDescriptor.EndpointMetadata.OfType<T>().Any();
    }
}
