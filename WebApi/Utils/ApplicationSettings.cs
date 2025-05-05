using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Models.DataModels;

namespace WebApi.Utils
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

        public static void UseStaticFiles(this WebApplication app, string folderName)
        {
            var staticFilePath = Path.Combine(app.Environment.ContentRootPath, folderName);
            if (!Directory.Exists(staticFilePath))
                Directory.CreateDirectory(staticFilePath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(staticFilePath),
                RequestPath = $"/{folderName}",
            });
        }
    }
}
