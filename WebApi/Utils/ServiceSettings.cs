using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.DataModels;
using Services;
using Services.Helpers;
using Services.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

namespace WebApi.Utils
{
    public static class ServiceSettings
    {
        public static void RegisterDependency(this IServiceCollection services)
        {
            // services
            services.AddScoped<IMemberService, MemberService>();

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // helpers
            services.AddSingleton<JwtHelper>();
            services.AddSingleton<CallApiHelper>();
        }

        public static void UseDbContext(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;

            builder.Services.AddDbContext<DataContext>(opt =>
            {
                if (config.HasConnectionString("SqlServer", out var sqlServerConnectionString))
                    opt.UseSqlServer(sqlServerConnectionString);
                else if (config.HasConnectionString("MySQL", out var mySqlConnectionString))
                    opt.UseMySql(mySqlConnectionString, new MySqlServerVersion(new Version(8, 0, 0)));
            });
        }

        private static bool HasConnectionString(this IConfiguration config, string name, out string connectionString)
        {
            connectionString = config.GetConnectionString(name);
            return !string.IsNullOrWhiteSpace(connectionString);
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.ApiVersionReader = new UrlSegmentApiVersionReader();
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
            services.AddSwaggerGen((option) =>
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                option.IncludeXmlComments(xmlPath);
                //option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer"
                //});

                //option.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference() { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                //        }, Array.Empty<string>()
                //    }
                //});
            });
        }

        public static void AddJwtVerification(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = false,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key")))
                    };
                });

            builder.Services.AddAuthorization();
        }
    }
}
