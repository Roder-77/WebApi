using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WebApi.Filters
{
    public class SwaggerSchemaEnumDescription : ISchemaFilter
    {
        private readonly XDocument _xmlComments;
        private readonly string? _assemblyName;

        public SwaggerSchemaEnumDescription(XDocument xmlComments)
        {
            _xmlComments = xmlComments;
            _assemblyName = DetermineAssembly(xmlComments);
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            if (!type.IsEnum)
                return;

            if (type.Assembly.GetName().Name != _assemblyName)
                return;

            var sb = new StringBuilder(schema.Description);

            sb.AppendLine("<p>Possible values:</p>");
            sb.AppendLine("<ul>");

            foreach (var name in Enum.GetValues(type))
            {
                var value = Convert.ToInt64(name);
                var fullName = $"F:{type.FullName}.{name}";
                var description = _xmlComments.XPathEvaluate($"normalize-space(//member[@name = '{fullName}']/summary/text())") as string;

                sb.AppendLine($"<li><b>{value} - {name}</b>: {description}</li>");
            }

            sb.AppendLine("</ul>");

            schema.Description = sb.ToString();
        }

        private string? DetermineAssembly(XDocument doc)
        {
            var name = ((IEnumerable)doc.XPathEvaluate("/doc/assembly")).Cast<XElement>().ToList().FirstOrDefault();

            return name?.Value;
        }
    }
}
