using Microsoft.EntityFrameworkCore;
using Models.DataModels;

namespace MVC.Utils
{
    public static class ApplicationSettings
    {
        public static void UseDbMigration(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.Migrate();
            }
        }
    }
}
