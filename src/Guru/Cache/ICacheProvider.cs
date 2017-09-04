using System;

namespace Guru.Cache
{
    public interface ICacheProvider
    {
         bool Set<T>(string key, T value);

         bool Set<T>(string key, T value, DateTime expireTime);

         T Get<T>(string key);

         T GetOrSet<T>(string key, SetDelegate<T> setDelegate);
    }
}