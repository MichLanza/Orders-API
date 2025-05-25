using StackExchange.Redis;
using System;

namespace DDD.OrdersApp.Infrastructure.Cache
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;
        public RedisCacheService(string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
        }

        public void SetString(string key, string value, TimeSpan? expiry = null)
        {
            _db.StringSet(key, value, expiry);
        }

        public string? GetString(string key)
        {
            return _db.StringGet(key);
        }
    }
}