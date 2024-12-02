using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;
        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task DeleteDataAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);
            if (data == null)
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetDataAsync<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(180)
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
        }
    }
}
