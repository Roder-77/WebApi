using Common.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Infrastructures
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> Get<T>(string key)
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes is null)
                return default;

            var json = await GZipHelper.Unzip(bytes);

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task Set<T>(string key, T value, Action<DistributedCacheEntryOptions>? action = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (action is not null)
                action(options);

            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            var bytes = await GZipHelper.Zip(json);
            await _cache.SetAsync(key, bytes, options);
        }

        public async Task Remove(string key)
            => await _cache.RemoveAsync(key);
    }
}