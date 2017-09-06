using System;
using System.Threading.Tasks;

namespace Guru.Cache
{
    public interface ICacheProvider
    {
        bool Set<T>(string key, T value);

        bool Set<T>(string key, T value, TimeSpan expiry);

        T Get<T>(string key);

        T GetOrSet<T>(string key, SetDelegate setDelegate);

        Task<T> GetAsync<T>(string key);

        Task<T> GetOrSetAsync<T>(string key, SetAsyncDelegate setDelegate);

        Task<bool> SetAsync<T>(string key, T value);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry);
    }
}