using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Models.Attritubes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebApi.Filters
{
    public class SwaggerSchemaIgnoreEnum : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum)
                return;

            var openApiStrings = new List<IOpenApiAny>();
            foreach (var value in Enum.GetValues(context.Type))
            {
                var member = context.Type.GetMember(value.ToString()).First();
                if (!member.GetCustomAttributes<SwaggerIgnoreEnum>().Any())
                    openApiStrings.Add(new OpenApiString(value.ToString()));
            }

            schema.Enum = openApiStrings;
        }
    }
}
