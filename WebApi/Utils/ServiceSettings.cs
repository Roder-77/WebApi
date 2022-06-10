using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Models.DataModels;
using Services;
using Services.Repositories;
using System.Reflection;

namespace WebApi.Utils
{
    public static class ServiceSettings
    {
        public static void RegisterDependency(this IServiceCollection services)
        {
            // services
            services.AddScoped<IMemberService, MemberService>();

            // Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen((option) =>
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                option.IncludeXmlComments(xmlPath);
                option.SwaggerDoc("v1", CresteOpenApiInfo("v1", "Web API"));
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

        public static OpenApiInfo CresteOpenApiInfo(string version, string title, string description = "")
        {
            return new OpenApiInfo()
            {
                Version = version,
                Title = title,
                Description = description,
                //Contact = new OpenApiContact() { Name = "標題", Email = "", Url = null },
                //TermsOfService = new Uri(""),
                //License = new OpenApiLicense() { Name = "文件", Url = new Uri("") }
            };
        }
    }
}
