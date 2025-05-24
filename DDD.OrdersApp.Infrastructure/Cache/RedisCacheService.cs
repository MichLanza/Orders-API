using StackExchange.Redis;

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
        public void SetString(string key, string value)
        {
            _db.StringSet(key, value);
        }
        public string? GetString(string key)
        {
            return _db.StringGet(key);
        }
    }
}
