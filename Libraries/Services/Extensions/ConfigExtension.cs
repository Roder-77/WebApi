using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Models;
using Models.DataModels;
using Models.Requests.Validators;
using Scrutor;
using Services.HostedServices;
using Services.JobServices;
using Services.Repositories;
using System.Reflection;
using static Services.Interface.IMailService;

namespace Services.Extensions
{
    public static class ConfigExtension
    {
        #region Utilities

        private static bool HasConnectionString(this IConfiguration config, string name, out string? connectionString)
        {
            connectionString = config.GetConnectionString(name);
            return !string.IsNullOrWhiteSpace(connectionString);
        }

        private static void BindConfiguration<TOptions>(this IServiceCollection services, string sectionName)
            where TOptions : class
        {
            services.AddOptions<TOptions>()
                    .BindConfiguration(sectionName)
                    .ValidateDataAnnotations();
        }

        #endregion

        #region IServiceCollection

        public static void AddService(this IServiceCollection services)
        {
            var baseServiceType = typeof(BaseService);

            services.Scan(scan =>
                scan.FromAssembliesOf(baseServiceType)
                    .AddClasses(classes => classes.AssignableTo(baseServiceType))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithScopedLifetime()
            );

            services.AddScoped<CommonService>();

            services.AddSingleton<CallApiService>();
            services.AddSingleton<JwtService>();
        }

        public static void AddHostedService(this IServiceCollection services)
            => services.AddHostedService<TestBackGroundService>();

        public static void AddMailService(this IServiceCollection services)
        {
            services.BindConfiguration<MailSettings>(nameof(MailSettings));
            services.AddScoped<AwsMailService>();
            services.AddTransient<MailServiceResolver>(serviceProvider => type =>
            {
                return type switch
                {
                    MailServiceType.Aws => serviceProvider.GetRequiredService<AwsMailService>()!,
                    _ => throw new NotSupportedException()
                };
            });
        }

        public static void AddJobService(this IServiceCollection services)
        {
            var baseServiceType = typeof(BaseJobService);

            services.Scan(scan =>
                scan.FromAssembliesOf(baseServiceType)
                    .AddClasses(classes => classes.AssignableTo(baseServiceType))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithScopedLifetime()
            );

            services.AddHostedService<DynamicHangfireService>();
        }

        public static void AddHangfire(this IServiceCollection services, string connectionString, string? schemaPrefix, bool enableServer = true)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    connectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true,
                        SchemaName = $"{schemaPrefix}Hangfire"
                    }));

            if (enableServer)
                services.AddHangfireServer();

            //services.AddHangfireServer(options => options.WorkerCount = 10);
        }

        public static void AddValidator(this IServiceCollection services)
        {
            ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();

            services.AddValidatorsFromAssemblyContaining<CustomLanguageManager>();
            //services.AddFluentValidationAutoValidation();
        }

        public static void AddConfigure(this IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = int.MaxValue);
        }

        #endregion

        #region WebApplicationBuilder

        public static void RegisterDependency(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddDbContext<DataContext>(options =>
            {
                if (builder.Configuration.HasConnectionString("SqlServer", out var sqlServerConnectionString))
                    options.UseSqlServer(sqlServerConnectionString, options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                else
                    throw new InvalidOperationException("Connection string 'SqlServer' is not found.");

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.MultipleNavigationProperties));
                }
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddService();

            services.AddMailService();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMapster();

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }

        public static void AddHangfire(this WebApplicationBuilder builder, bool enableServer = true)
        {
            if (!builder.Configuration.HasConnectionString("SqlServer", out var connectionString))
                throw new InvalidOperationException("Connection string 'Hangfire' is not found.");

            var schemaPrefix = builder.Configuration["Hangfire:SchemaPrefix"];
            builder.Services.AddHangfire(connectionString!, schemaPrefix, enableServer);
        }

        public static void AddDefaultCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin => {
                            var allowedHosts = builder.Configuration.GetValue<string>("AllowedHosts");
                            if (string.IsNullOrWhiteSpace(allowedHosts))
                                return false;

                            if (allowedHosts == "*")
                                return true;

                            var originHost = new Uri(origin).Host;
                            var allowedHostParts = allowedHosts.Split(';');

                            return allowedHostParts.Any(host => originHost.Equals(host, StringComparison.OrdinalIgnoreCase));
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static void UseDbMigration(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            db.Database.Migrate();
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

        #endregion
    }
}
