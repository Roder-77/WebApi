using Hangfire;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.DataModels;
using Models.JobModels;
using Services.JobServices;
using Services.Repositories;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Services.HostedServices
{
    public class DynamicHangfireService : BackgroundService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IServiceProvider _serviceProvider;

        private readonly IGenericRepository<JobSettings> _repo;
        private readonly ILogger<DynamicHangfireService> _logger;

        private readonly ConcurrentDictionary<string, Type> _typeCache = new();
        private HashSet<JobModel> _jobs = [];

        public DynamicHangfireService(
            IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider,
            IGenericRepository<JobSettings> repository,
            ILogger<DynamicHangfireService> logger)
        {
            _recurringJobManager = recurringJobManager;
            _serviceProvider = serviceProvider;
            _repo = repository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    HandleJob();
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DynamicHangfireService)} Fail");
                throw;
            }
        }

        private async Task HandleJob()
        {
            // var job = new JobModel
            // {
            //     Id = "jobId",
            //     DisplayName = "My Job",
            //     ServiceName = "MyService",
            //     MethodName = "DoSomething",
            //     CronExpression = "30 02 * * *",
            //     Arguments = ["123", 456],
            //     IsActive = true,
            //     TimeZone = "Asia/Taipei"
            // };
            var settings = await _repo.Query(x => x.IsActive).ToListAsync();
            var jobs = settings.Adapt<List<JobModel>>();

            foreach (var job in jobs)
            {
                if (_jobs.Contains(job))
                {
                    _logger.LogInformation($"Job {job.Id} is already registered and unchanged.");
                    continue;
                }

                _recurringJobManager.RemoveIfExists(job.Id);
                _jobs.RemoveWhere(j => j.Id == job.Id);

                if (!job.IsEnabled)
                {
                    _logger.LogInformation($"Job {job.Id} is disabled and removed from the list.");
                    continue;
                }

                _jobs.Add(job);
                RegisterJob(job);
                _logger.LogInformation($"Job {job.Id} has been added or updated.");
            }
        }

        private void RegisterJob(JobModel job)
        {
            try
            {
                var serviceType = _typeCache.GetOrAdd(job.ServiceName, key =>
                {
                    var baseType = typeof(BaseJobService);
                    var derivedType = baseType.Assembly.GetTypes()
                        .FirstOrDefault(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t.Name == key);

                    return derivedType ?? throw new TypeLoadException($"Type '{key}' not found.");
                });

                var methodInfo = serviceType.GetMethod(job.MethodName) ?? throw new MissingMethodException(job.ServiceName, job.MethodName);
                var service = _serviceProvider.GetRequiredService(serviceType);
                var methodParams = methodInfo.GetParameters();
                var args = methodParams.Select((p, i) => ConvertArg(job.Arguments[i], p.ParameterType)).ToArray();

                _recurringJobManager.AddOrUpdate(
                    job.Id,
                    job.DisplayName ?? job.Id,
                    () => methodInfo.Invoke(service, args),
                    job.CronExpression,
                    new() { TimeZone = TimeZoneInfo.FindSystemTimeZoneById(job.TimeZone) }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to register job {job.Id}");
            }
        }

        private object? ConvertArg(object? arg, Type targetType)
        {
            if (arg is null)
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            // 基本型別或可直接指派
            if (targetType.IsAssignableFrom(arg.GetType()))
            {
                return arg;
            }

            // Nullable<T>
            var underlying = Nullable.GetUnderlyingType(targetType);
            if (underlying is not null)
            {
                return ConvertArg(arg, underlying);
            }

            // 可用 Convert.ChangeType 的情況
            if (targetType.IsPrimitive ||
                targetType == typeof(decimal) ||
                targetType == typeof(string) ||
                targetType == typeof(DateTime) ||
                targetType == typeof(Guid))
            {
                return Convert.ChangeType(arg, targetType);
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 複合型別：從 JSON 反序列化
            if (arg is string s)
            {
                return JsonSerializer.Deserialize(s, targetType, options)!;
            }

            // 若來源是字典/匿名物件，可先轉為 JSON 再反序列化
            var json = JsonSerializer.Serialize(arg);
            return JsonSerializer.Deserialize(json, targetType, options)!;
        }
    }
}
