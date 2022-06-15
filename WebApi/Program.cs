using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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

    builder.Services.AddHttpClient();

    builder.Services.RegisterDependency();

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services
        .AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer()));
            options.Filters.Add(typeof(LogInvalidModelState));
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            // Disable automatic 400 response
            options.SuppressModelStateInvalidFilter = true;
        });

    // DbContext
    //builder.UseDbContext();
    builder.Services.AddDbContext<MemoryContext>(options => options.UseInMemoryDatabase("MemoryDemo"));

    builder.Services.AddSwagger();

    builder.AddJwtVerification();

    // Serilog
    builder.WebHost.UseSerilog();

    var app = builder.Build();

    //app.ApplyDbMigration();

    app.UseApiVersioning();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            options.SwaggerEndpoint(url, description.GroupName.ToUpperInvariant());
        }
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