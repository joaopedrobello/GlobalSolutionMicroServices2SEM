using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.Services
{
    public class CacheService
    {
        private readonly IDatabase _cache;
        private readonly TimeSpan _cacheDuration;

        public CacheService(IConfiguration configuration)
        {
            var redisConnection = configuration.GetValue<string>("RedisSettings:ConnectionString");
            var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnection);
            _cache = connectionMultiplexer.GetDatabase();
            _cacheDuration = TimeSpan.FromSeconds(configuration.GetValue<int>("RedisSettings:CacheDurationSeconds"));
        }

        public async Task SetCacheAsync(string key, object value)
        {
            var serializedValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            await _cache.StringSetAsync(key, serializedValue, _cacheDuration);
        }

        public async Task<T> GetCacheAsync<T>(string key)
        {
            var cachedValue = await _cache.StringGetAsync(key);
            if (cachedValue.IsNullOrEmpty)
            {
                return default;
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cachedValue);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _cache.KeyExistsAsync(key);
        }
    }
}
