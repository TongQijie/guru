namespace Guru.Redis.Configuration
{
    public interface IRedisConfiguration
    {
        string Connection { get; }

        int DbIndex { get; }
    }
}
