using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using Services;
using Services.Repositories;

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
    }
}
