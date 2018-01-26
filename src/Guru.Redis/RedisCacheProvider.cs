using System;
using System.Text;
using System.Threading.Tasks;

using Guru.Cache;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection;
using Guru.Redis.Configuration;
using Guru.Logging.Abstractions;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;

using StackExchange.Redis;
using Guru.Logging;
using Guru.Cache.Delegates;

namespace Guru.Redis
{
    [Injectable(typeof(IRedisCacheProvider), Lifetime.Singleton)]
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IRedisConfiguration _RedisConfiguration;

        private readonly ILogger _Logger;

        private readonly IFormatter _Formatter;

        private ConnectionMultiplexer _ConnectionMultiplexer;

        private IDatabase _Databse;

        public RedisCacheProvider(IRedisConfiguration redisConfiguration, IFileLogger fileLogger, IJsonFormatter jsonFormatter)
        {
            _RedisConfiguration = redisConfiguration;
            _Logger = fileLogger;
            _Formatter = jsonFormatter;
        }

        private IDatabase GetDatabase()
        {
            if (!_RedisConfiguration.Connection.HasValue())
            {
                _Logger.LogEvent("RedisCacheProvider", Severity.Error, "failed to connect to redis server.", "connection string is empty.");
                return null;
            }

            if (_ConnectionMultiplexer == null || !_ConnectionMultiplexer.IsConnected)
            {
                try
                {
                    _ConnectionMultiplexer = ConnectionMultiplexer.Connect(_RedisConfiguration.Connection);
                }
                catch (Exception e)
                {
                    _Logger.LogEvent("RedisCacheProvider", Severity.Error, "failed to connect to redis server.", _RedisConfiguration.Connection, e);
                    return null;
                }

                _Databse = null;
            }

            if (_Databse == null)
            {
                _Databse = _ConnectionMultiplexer.GetDatabase(_RedisConfiguration.DbIndex);
            }

            return _Databse;
        }

        public T Get<T>(string key)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return default(T);
            }

            if (!db.KeyExists(key))
            {
                return default(T);
            }

            return _Formatter.ReadObject<T>(db.StringGet(key), Encoding.UTF8);
        }

        public T GetOrSet<T>(string key, SetDelegate<T> setDelegate)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return default(T);
            }

            if (!db.KeyExists(key))
            {
                setDelegate(this);
            }

            if (!db.KeyExists(key))
            {
                return default(T);
            }

            return _Formatter.ReadObject<T>(db.StringGet(key), Encoding.UTF8);
        }

        public bool Set<T>(string key, T value, TimeSpan expiry)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return false;
            }

            return db.StringSet(key, _Formatter.WriteString(value, Encoding.UTF8), expiry);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return default(T);
            }

            if (!db.KeyExists(key))
            {
                return default(T);
            }

            return await _Formatter.ReadObjectAsync<T>(await db.StringGetAsync(key), Encoding.UTF8);
        }

        public async Task<T> GetOrSetAsync<T>(string key, SetAsyncDelegate<T> setAsyncDelegate)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return default(T);
            }

            if (!db.KeyExists(key))
            {
                await setAsyncDelegate(this);
            }

            if (!db.KeyExists(key))
            {
                return default(T);
            }

            return await _Formatter.ReadObjectAsync<T>(await db.StringGetAsync(key), Encoding.UTF8);
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return false;
            }

            return await db.StringSetAsync(key, await _Formatter.WriteStringAsync(value, Encoding.UTF8), expiry);
        }

        public T GetOrSet<T>(string key, SetDelegate<T> setDelegate, TimeSpan expiry)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return default(T);
            }

            if (db.KeyExists(key))
            {
                return Get<T>(key);
            }
            else
            {
                var value = setDelegate(this);
                if (value == null)
                {
                    return default(T);
                }

                if (Set(key, value, expiry))
                {
                    return value;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public bool Remove(string key)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return false;
            }

            return db.KeyDelete(key);
        }

        public async Task<T> GetOrSetAsync<T>(string key, SetAsyncDelegate<T> setDelegate, TimeSpan expiry)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return default(T);
            }

            if (await db.KeyExistsAsync(key))
            {
                return await GetAsync<T>(key);
            }
            else
            {
                var value = setDelegate(this).GetAwaiter().GetResult();
                if (value == null)
                {
                    return default(T);
                }

                if (await SetAsync(key, value, expiry))
                {
                    return value;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            var db = GetDatabase();
            if (db == null)
            {
                return false;
            }

            return await db.KeyDeleteAsync(key);
        }
    }
}