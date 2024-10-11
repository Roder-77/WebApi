using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.DataModels;
using Serilog;
using Services.Extensions;
using Services.Repositories;

try
{
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateBootstrapLogger();

    Log.Information("Starting web host");

    var connectionString = configuration.GetConnectionString("SqlServer");
    var builder = Host.CreateDefaultBuilder(args);

    builder.ConfigureServices(services =>
    {
        services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddService();

        services.AddMailService();

        //services.AddHangfire(connectionString);

        services.AddHttpContextAccessor();
    });

    builder.UseSerilog();

    using var host = builder.Build();

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}