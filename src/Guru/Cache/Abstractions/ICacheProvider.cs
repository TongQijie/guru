using System;
using System.Threading.Tasks;

using Guru.Cache.Delegates;

namespace Guru.Cache.Abstractions
{
    public interface ICacheProvider
    {
        T Get<T>(string key);

        bool Set<T>(string key, T value, TimeSpan expiry);

        T GetOrSet<T>(string key, SetDelegate<T> setDelegate, TimeSpan expiry);

        bool Remove(string key);

        Task<T> GetAsync<T>(string key);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry);

        Task<T> GetOrSetAsync<T>(string key, SetAsyncDelegate<T> setDelegate, TimeSpan expiry);

        Task<bool> RemoveAsync(string key);
    }
}