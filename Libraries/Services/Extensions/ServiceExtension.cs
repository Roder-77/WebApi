using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Scrutor;
using Services.HostedServices;
using Services.Interface;
using Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.Interface.IMailService;

namespace Services.Extensions
{
    public static class ServiceExtension
    {
        public static void AddService(this IServiceCollection services)
        {
            var baseServiceType = typeof(BaseService<>);

            services.Scan(scan =>
                scan.FromAssembliesOf(baseServiceType)
                    .AddClasses(classes => classes.AssignableTo(baseServiceType))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithScopedLifetime()
            );

            services.AddSingleton<JwtService>();
            services.AddSingleton<CallApiService>();
            services.AddSingleton<CommonService>();
        }

        public static void AddRepository(this IServiceCollection services)
            => services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

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
    }
}
