﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;
using Services.Extensions;
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

    builder.Services.AddConfigure();

    builder.Services.AddDistributedMemoryCache();

    //builder.Services.AddDbContext<MemoryContext>(options => options.UseInMemoryDatabase("MemoryDemo"));

    builder.Services
        .AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new CamelCaseParameterTransformer()));
            options.Filters.Add(typeof(LogInvalidModelState));
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            // Disable automatic 400 response
            options.SuppressModelStateInvalidFilter = true;
        });

    builder.Services.AddValidator();

    builder.Services.AddSwagger();

    builder.AddDefaultCors();

    builder.RegisterDependency();

    // Serilog
    builder.Host.UseSerilog();

    var app = builder.Build();

    app.UseDbMigration();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Disable swagger schemas at bottom
            options.DefaultModelsExpandDepth(-1);

            foreach (var description in app.DescribeApiVersions())
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        });

        app.MapGet("/", () => Results.Redirect("/swagger/index.html", permanent: false))
           .ExcludeFromDescription();
    }

    app.UseMiddleware<LogApiInformation>();

    //app.UseHttpsRedirection();
    app.UseRouting();

    app.UseCors();

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