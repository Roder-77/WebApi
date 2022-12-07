using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Models;
using Models.DataModels;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;
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

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddHttpClient();

    builder.Services.Configure<Appsettings>(builder.Configuration);

    builder.Services.AddDistributedMemoryCache();

    //builder.Services.AddDbContext<MemoryContext>(options => options.UseInMemoryDatabase("MemoryDemo"));

    builder.Services
        .AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new CamelCaseParameterTransformer()));
            options.Filters.Add(typeof(LogInvalidModelState));
        })
        .AddJsonOptions(options =>options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
        .ConfigureApiBehaviorOptions(options =>
        {
            // Disable automatic 400 response
            options.SuppressModelStateInvalidFilter = true;
        });

    builder.Services.AddSwagger();

    builder.AddJwtVerification();

    builder.AddDefaultCors();

    builder.RegisterDependency();

    // Serilog
    builder.Host.UseSerilog();

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

    app.UseStaticFiles("upload");

    app.UseCors();

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