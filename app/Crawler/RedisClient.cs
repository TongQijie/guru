using StackExchange.Redis;

namespace Crawler
{
    public class RedisClient
    {
        private static RedisClient _Current = null;

        public static RedisClient Current { get { return _Current ?? (_Current = new RedisClient()); }}

        private ConnectionMultiplexer _Redis;

        private IDatabase _Db;

        private RedisClient()
        {
            _Redis = ConnectionMultiplexer.Connect("localhost:6379");
            _Db = _Redis.GetDatabase();
        }

        public void Set(string key, string value)
        {
            _Db.SetAdd(key, value);
        }

        public string Get(string key)
        {
            return _Db.StringGet(key);
        }
    }
}