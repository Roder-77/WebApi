using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WebApi.Filters
{

    /// <summary>
    /// Represents the OpenAPI/Swashbuckle operation filter used to document information provided, but not used.
    /// </summary>
    /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
    /// Once they are fixed and published, this class can be removed.</remarks>
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated |= apiDescription.IsDeprecated();

            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();

                if (operation.Responses is null ||
                    !operation.Responses.TryGetValue(responseKey, out var response) ||
                    response.Content is null)
                {
                    continue;
                }

                foreach (var contentType in response.Content.Keys)
                {
                    if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
                        response.Content.Remove(contentType);
                }
            }

            if (operation.Parameters is null)
                return;

            for (var i = 0; i < operation.Parameters.Count; i++)
            {
                var parameter = operation.Parameters[i];
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                parameter.Description ??= description.ModelMetadata?.Description;

                JsonNode? defaultValue = null;
                if (parameter.Schema?.Default is null &&
                     description.DefaultValue is not null &&
                     description.DefaultValue is not DBNull &&
                     description.ModelMetadata is ModelMetadata modelMetadata)
                {
                    var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                    defaultValue = JsonNode.Parse(json);
                }

                var needsUpdate = defaultValue is not null || (description.IsRequired && !parameter.Required);
                if (needsUpdate)
                {
                    var newSchema = new OpenApiSchema
                    {
                        Type = parameter.Schema?.Type,
                        Format = parameter.Schema?.Format,
                        Default = defaultValue ?? parameter.Schema?.Default
                    };

                    operation.Parameters[i] = new OpenApiParameter
                    {
                        Name = parameter.Name,
                        In = parameter.In,
                        Description = parameter.Description,
                        Required = description.IsRequired || parameter.Required,
                        Schema = newSchema,
                        Example = parameter.Example,
                        Examples = parameter.Examples,
                        Style = parameter.Style,
                        Explode = parameter.Explode,
                        AllowEmptyValue = parameter.AllowEmptyValue,
                        AllowReserved = parameter.AllowReserved,
                        Deprecated = parameter.Deprecated,
                        Content = parameter.Content
                    };
                }
            }
        }
    }
}
