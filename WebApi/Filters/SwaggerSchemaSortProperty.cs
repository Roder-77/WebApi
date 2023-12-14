using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Runtime.Serialization;

namespace WebApi.Filters
{
    public class SwaggerSchemaSortProperty : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var properties = context.Type.GetProperties();
            if (!properties.Any(x => IsDefinedDataMemberAttribute(x)))
                return;

            var dic = new Lazy<Dictionary<string, OpenApiSchema>>();
            var exceptProperties = properties.Where(x => !IsDefinedDataMemberAttribute(x));
            var orderProperties = properties
                .Where(x => IsDefinedDataMemberAttribute(x))
                .OrderBy(x => x.GetCustomAttribute<DataMemberAttribute>(true)!.Order)
                .ToList();

            orderProperties.AddRange(exceptProperties);

            foreach (var property in orderProperties)
            {
                var current = schema.Properties.FirstOrDefault(x => x.Key.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                ((ICollection<KeyValuePair<string, OpenApiSchema>>)dic.Value).Add(current);
            }

            schema.Properties = dic.Value;
        }

        private bool IsDefinedDataMemberAttribute(PropertyInfo propertyInfo)
            => Attribute.IsDefined(propertyInfo, typeof(DataMemberAttribute), true);
    }
}
