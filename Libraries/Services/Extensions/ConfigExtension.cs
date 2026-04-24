using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Models.DataModels;
using Models.Infrastructures;
using Models.Requests.Validators;
using Scrutor;
using Serilog;
using Serilog.Events;
using Services.Filters;
using Services.HostedServices;
using Services.Infrastructures;
using Services.Infrastructures.JobServices;
using Services.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using static Services.Infrastructures.Interface.IMailService;

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

        private static bool IsHealthCheck(LogEvent logEvent)
        {
            if (logEvent.Properties.TryGetValue("RequestPath", out var requestPathValue) &&
                requestPathValue is ScalarValue requestPathScalar)
            {
                var requestPath = requestPathScalar.Value as string;

                return !string.IsNullOrEmpty(requestPath) && requestPath.StartsWith("/hc", StringComparison.OrdinalIgnoreCase);
            }

            return false;
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
                .UseConsole()
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

        public static void AddApiVersion(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("X-Api-Version"));
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        public static void AddScalar(this IServiceCollection services, IEnumerable<OpenApiParameter>? headers = null)
        {
            services.AddApiVersion();

            var apiVersionDescriptionProvider = services
                .BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                services.AddOpenApi(description.GroupName, options =>
                {
                    options.AddDocumentTransformer((document, context, ct) =>
                    {
                        document.Info.Title = $"My API {description.ApiVersion}";
                        document.Info.Description = $"版本 {description.GroupName} 的文件";
                        return Task.CompletedTask;
                    });

                    // 排序 Response 欄位
                    options.AddSchemaTransformer<OpenApiSchemaSortProperty>();

                    // 加入 Header 參數
                    options.AddOperationTransformer(new OpenApiHeaderParameter(headers));
                });
            }
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddApiVersion();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                var xmlFiles = new List<string>
                {
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
                    $"{Assembly.GetAssembly(typeof(DataContext))!.GetName().Name}.xml"
                };

                foreach (var xmlFile in xmlFiles)
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

                options.OrderActionsBy(apiDesc => apiDesc.RelativePath);
                options.SchemaFilter<SwaggerSchemaSortProperty>();
                options.OperationFilter<SwaggerIgnoreParameter>();
                options.OperationFilter<SwaggerDefaultValues>();

                options.UseInlineDefinitionsForEnums();
                options.EnableAnnotations();
                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme.",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});

                //options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference() { Id = "Bearer", Type = ReferenceType.SecurityScheme },
                //        }, Array.Empty<string>()
                //    }
                //});
            });
        }

        public static void AddJwtVerification(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key")!))
                    };
                });

            builder.Services.AddAuthorization();
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

        public static LoggerConfiguration SetLoggerConfiguration(bool isDevelopment, string prefix)
        {
            var machineName = Environment.MachineName;
            var outputTemplate = "=================================================={NewLine}{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}{NewLine}requestId:[{RequestId}] [{Level}]{NewLine}{SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}";
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.ViewFeatures", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.StaticFiles", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Filter.ByExcluding(IsHealthCheck)
                .WriteTo.Console();

            if (isDevelopment)
            {
                loggerConfiguration = loggerConfiguration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                    .WriteTo.File(
                        path: $"logs/log_{machineName}_.txt",
                        rollingInterval: RollingInterval.Hour,
                        outputTemplate: outputTemplate
                    );
            }
            else
            {
                loggerConfiguration = loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .WriteTo.File(
                        path: $"/mnt/files/goif_logs/{prefix}/log_{machineName}_.txt",
                        rollingInterval: RollingInterval.Hour,
                        outputTemplate: outputTemplate
                    );
            }

            return loggerConfiguration;
        }
    }
}
