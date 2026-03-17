using Microsoft.OpenApi;
using Models.Attritubes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Nodes;

namespace WebApi.Filters
{
    public class SwaggerSchemaIgnoreEnum : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum)
                return;

            var openApiStrings = new Lazy<List<JsonNode>>();
            foreach (var value in Enum.GetValues(context.Type))
            {
                var member = context.Type.GetMember(value.ToString()!).First();
                if (!member.GetCustomAttributes<SwaggerIgnoreEnum>().Any())
                    openApiStrings.Value.Add(new OpenApiString(value.ToString()));
            }

            schema.Enum = openApiStrings.Value;
        }
    }
}
