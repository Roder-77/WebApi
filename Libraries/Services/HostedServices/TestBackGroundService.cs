using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services.HostedServices
{
    public class TestBackGroundService : IHostedService, IDisposable
    {
        private Timer? _timer;

        private readonly ILogger<TestBackGroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TestBackGroundService(ILogger<TestBackGroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var dueTime = new DateTime(now.Year, now.Month, now.Day + 1, 1, 0, 0) - now;
            var period = TimeSpan.FromHours(24);

            _timer = new Timer(new TimerCallback(DoWork), null, dueTime, period);
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var service = nameof(TestBackGroundService);

            _logger.LogInformation($"{service} Start");

            try
            {
                //using var scope = _scopeFactory.CreateScope();
                //var service = scope.ServiceProvider.GetRequiredService<>();

                _logger.LogInformation($"{service} End");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{service} Error");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
