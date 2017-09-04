using Guru.DependencyInjection.Attributes;

namespace Guru.Redis.Configuration
{
    [StaticFile(typeof(IRedisConfiguration), "./Configuration/redis.json")]
    public class RedisConfiguration : IRedisConfiguration
    {
        public RedisConfiguration()
        {
            DbIndex = -1;
        }

        public string Connection { get; set; }

        public int DbIndex { get; set; }
    }
}
