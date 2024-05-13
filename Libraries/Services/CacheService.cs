using Common.Helpers;
using Microsoft.Extensions.Caching.Distributed;
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

        public async Task<T?> Get<T>(string key)
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes is null)
                return default;

            var json = await GZipHelper.Unzip(bytes);

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task Set<T>(string key, T value, DistributedCacheEntryOptions? options = null)
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = await GZipHelper.Zip(json);

            if (options is null)
                await _cache.SetAsync(key, bytes);
            else
                await _cache.SetAsync(key, bytes, options);
        }

        public async Task Remove(string key)
            => await _cache.RemoveAsync(key);
    }
}
