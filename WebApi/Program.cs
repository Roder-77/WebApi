using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi.Data;
using WebApi.Extensions;
using WebApi.Utils;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    // services

    // Repository

    builder.Services.AddControllers(opt =>
    {
        opt.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer()));
    });

    // DbContext
    builder.Services.AddDbContext<DataContext>(opt =>
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen((opt) =>
    {
        opt.IncludeXmlComments(builder.Configuration["XmlCommentPath"]);
    });

    // Serilog
    builder.WebHost.UseSerilog();

    var app = builder.Build();

    app.ApplyDbMigration();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseExceptionHandler("/error");

    app.UseHttpsRedirection();

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