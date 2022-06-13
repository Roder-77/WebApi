using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using Serilog;
using System.Reflection;
using WebApi.Filters;
using WebApi.Middleware;
using WebApi.Utils;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    Log.Information("Starting web host");

    builder.Services.RegisterDependency();

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddHttpClient();

    builder.Services.AddControllers(config =>
    {
        config.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer()));
        config.Filters.Add(typeof(LogInvalidModelState));
    });

    // DbContext
    //builder.UseDbContext();
    builder.Services.AddDbContext<MemoryContext>(option => option.UseInMemoryDatabase("MemoryDemo"));

    builder.Services.AddSwagger();

    // Serilog
    builder.WebHost.UseSerilog();

    var app = builder.Build();

    //app.ApplyDbMigration();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    app.UseMiddleware<LogApiInformation>();

    //app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}