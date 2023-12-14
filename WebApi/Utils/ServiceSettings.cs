using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models;
using Models.DataModels;
using Services.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using WebApi.Filters;

namespace WebApi.Utils
{
    public static class ServiceSettings
    {
        public static void RegisterDependency(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddRepository(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("SqlServer"),
                    options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                );

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.MultipleNavigationProperties));
                }
            });

            services.AddService();
            //services.AddHostedService();
            services.AddMailService();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureApiVersionSwaggerGenOptions>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var xmlFiles = new List<string>
                {
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
                    $"{Assembly.GetAssembly(typeof(DataContext))!.GetName().Name}.xml"
                };

                foreach (var xmlFile in xmlFiles)
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

                options.OrderActionsBy(apiDesc => apiDesc.RelativePath);
                options.SchemaFilter<SwaggerSchemaSortProperty>();
                options.OperationFilter<SwaggerIgnoreParameter>();

                options.UseInlineDefinitionsForEnums();
                options.EnableAnnotations();
                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme.",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});

                //options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference() { Id = "Bearer", Type = ReferenceType.SecurityScheme },
                //        }, Array.Empty<string>()
                //    }
                //});
            });
        }

        public static void AddJwtVerification(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddOptions<Jwtsettings>()
                .Configure<IConfiguration>((settings, config) => config.GetSection("JwtSettings").Bind(settings));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key")!))
                    };
                });

            builder.Services.AddAuthorization();
        }

        public static void AddDefaultCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin => {
                            var allowedHosts = builder.Configuration.GetValue<string>("AllowedHosts");
                            if (allowedHosts == "*")
                                return true;

                            var originHost = new Uri(origin).Host;
                            var allowedHostParts = allowedHosts!.Split(';');

                            return allowedHostParts.Any(host => originHost.Equals(host, StringComparison.OrdinalIgnoreCase));
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }
}
