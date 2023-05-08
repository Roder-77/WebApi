using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

namespace WebApi.Filters
{
    public class SwaggerIgnoreParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null || context.ApiDescription?.ParameterDescriptions == null)
                return;

            var descriptions = context.ApiDescription.ParameterDescriptions.Where(x =>
                (x.Source.Equals(BindingSource.Query) || x.Source.Equals(BindingSource.Form)) &&
                x.CustomAttributes().Any(y => y.GetType().Equals(typeof(JsonIgnoreAttribute))));

            if (descriptions.Any())
                operation.Parameters = operation.Parameters.Where(x => !descriptions.Any(y => y.Name == x.Name)).ToList();
        }
    }
}
