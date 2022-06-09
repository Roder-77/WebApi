using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using Serilog;
using System.Reflection;
using WebApi.Utils;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    Log.Information("Starting web host");

    builder.Services.AddHttpClient();

    builder.Services.RegisterDependency();

    builder.Services.AddControllers(opt =>
    {
        opt.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer()));
    });

    // DbContext
    //builder.UseDbContext();
    builder.Services.AddDbContext<MemoryContext>(opt => opt.UseInMemoryDatabase("MemoryDemo"));

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