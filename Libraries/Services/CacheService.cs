using Common.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Services
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> Get<T>(string key) where T : class
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes is null)
                return default;

            var json = GZipHelper.Unzip(bytes);

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task Set<T>(string key, T value, DistributedCacheEntryOptions? options = null) where T : class
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = GZipHelper.Zip(json);

            if (options is not null)
            {
                await _cache.SetAsync(key, bytes, options);
                return;
            }

            await _cache.SetAsync(key, bytes);
        }

        public async Task Remove(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
