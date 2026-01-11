using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Extensions;

namespace Services.JobServices
{
    public class BaseJobService
    {
        protected readonly HttpContext _httpContext;
        protected readonly PerformContext _performContext;

        protected readonly ILogger<BaseJobService> _logger;

        public BaseJobService(IServiceProvider serviceProvider)
        {
            _httpContext = serviceProvider.SetDefaultHttpContext().HttpContext!;
            _performContext = serviceProvider.GetRequiredService<PerformContext>();
            _logger = serviceProvider.GetRequiredService<ILogger<BaseJobService>>();
        }

    }
}
