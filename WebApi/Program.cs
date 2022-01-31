using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi.DataModel;
using WebApi.Repositories;
using WebApi.Services;
using WebApi.Utils;

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    // services
    builder.Services.AddTransient<IMemberService, MemberService>();

    // Repository
    builder.Services.AddTransient<IMemberRepository, MemberRepository>();

    builder.Services.AddControllers(opt =>
    {
        opt.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer()));
    });

    // DbContext
    builder.Services.AddDbContext<MemoryContext>(opt => opt.UseInMemoryDatabase("MemoryDemo"));

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Serilog
    builder.WebHost.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseExceptionHandler("/error");

    app.UseHttpsRedirection();

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