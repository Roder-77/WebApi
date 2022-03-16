using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Extensions
{
    public static class ApplicationExtension
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
