using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using WebApi.Data;
using WebApi.DataModel;
using WebApi.Extensions;
using WebApi.Repositories;
using WebApi.Services;
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

    builder.Services.AddDependencyInject();

    builder.Services.AddControllers(opt =>
    {
        opt.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer()));
    });

    // DbContext
    builder.Services.AddDbContext<MemoryContext>(opt => opt.UseInMemoryDatabase("MemoryDemo"));
    //builder.Services.AddDbContext<DataContext>(opt =>
    //{
    //    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    //});

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen((opt) =>
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        opt.IncludeXmlComments(xmlPath);
    });

    // Serilog
    builder.WebHost.UseSerilog();

    var app = builder.Build();

    //app.ApplyDbMigration();

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