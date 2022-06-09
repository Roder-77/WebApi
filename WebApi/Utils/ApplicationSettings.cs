using Microsoft.EntityFrameworkCore;
using Models.DataModels;

namespace WebApi.Utils
{
    public static class ApplicationSettings
    {
        public static void ApplyDbMigration(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.Migrate();
            }
        }
    }
}
