using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Reflection;
using System.Runtime.Serialization;

namespace Services.Filters
{
    public class OpenApiSchemaSortProperty : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            if (schema.Properties is null || schema.Properties.Count == 0)
                return Task.CompletedTask;

            var properties = context.JsonTypeInfo.Type.GetProperties();
            if (!properties.Any(x => IsDefinedDataMemberAttribute(x)))
                return Task.CompletedTask;

            var sortedProperties = new Dictionary<string, IOpenApiSchema>();
            var exceptProperties = properties.Where(x => !IsDefinedDataMemberAttribute(x));
            var orderProperties = properties
                .Where(x => IsDefinedDataMemberAttribute(x))
                .OrderBy(x => x.GetCustomAttribute<DataMemberAttribute>(true)!.Order)
                .ToList();

            orderProperties.AddRange(exceptProperties);

            foreach (var property in orderProperties)
            {
                var current = schema.Properties.FirstOrDefault(x => x.Key.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(current.Key))
                    sortedProperties.Add(current.Key, current.Value);
            }

            schema.Properties.Clear();
            foreach (var kvp in sortedProperties)
            {
                schema.Properties.Add(kvp.Key, kvp.Value);
            }

            return Task.CompletedTask;
        }

        private static bool IsDefinedDataMemberAttribute(PropertyInfo propertyInfo)
            => Attribute.IsDefined(propertyInfo, typeof(DataMemberAttribute), true);
    }
}
