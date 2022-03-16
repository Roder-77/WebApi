using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseDbContext(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;

            builder.Services.AddDbContext<DataContext>(opt => {
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
