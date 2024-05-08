using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Utils
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var desc = string.Empty; // costomize

            if (description.IsDeprecated)
                desc += "(deprecated)";

            return new OpenApiInfo()
            {
                Version = description.ApiVersion.ToString(),
                Title = "Web API",
                Description = desc,
                //Contact = new OpenApiContact() { Name = "標題", Email = "", Url = null },
                //TermsOfService = new Uri(""),
                //License = new OpenApiLicense() { Name = "文件", Url = new Uri("") }
            };
        }
    }
}
