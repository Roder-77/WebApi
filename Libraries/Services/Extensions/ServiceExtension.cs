using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Models.DataModels;
using Models.Request.Validators;
using Scrutor;
using Services.HostedServices;
using Services.Repositories;
using static Services.Interface.IMailService;

namespace Services.Extensions
{
    public static class ServiceExtension
    {
        private static bool HasConnectionString(this IConfiguration config, string name, out string? connectionString)
        {
            connectionString = config.GetConnectionString(name);
            return !string.IsNullOrWhiteSpace(connectionString);
        }

        public static void RegisterDependency(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddDbContext<DataContext>(options =>
            {
                if (builder.Configuration.HasConnectionString("SqlServer", out var sqlServerConnectionString))
                    options.UseSqlServer(sqlServerConnectionString, options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                else if (builder.Configuration.HasConnectionString("MySQL", out var mySqlConnectionString))
                    options.UseMySql(mySqlConnectionString, new MySqlServerVersion(new Version(8, 0, 0)));

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
        }

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
            services.AddOptions<MailSettings>()
                .Configure<IConfiguration>((settings, config) => config.GetSection("Mail").Bind(settings));

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

        public static void AddHangfire(this IServiceCollection services, string connectionString)
        {
            services.AddHangfire(config =>
            {
                var storageOptions = new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                };
                var storage = new SqlServerStorage(connectionString, storageOptions);

                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseStorage(storage);
            });

            services.AddHangfireServer(options => options.WorkerCount = 10);
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

        public static void AddValidator(this IServiceCollection services)
        {
            ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CustomLanguageManager>();
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

            //services.AddOptions<>()
            //    .Configure<IConfiguration>((settings, config) => config.GetSection("").Bind(settings));
        }
    }
}
