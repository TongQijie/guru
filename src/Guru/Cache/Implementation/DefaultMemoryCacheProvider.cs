using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Guru.Cache.Delegates;
using Guru.ExtensionMethod;
using Guru.Cache.Abstractions;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.Logging;
using Guru.Executable.Abstractions;

namespace Guru.Cache.Implementation
{
    [Injectable(typeof(IMemoryCacheProvider), Lifetime.Singleton)]
    internal class DefaultMemoryCacheProvider : IMemoryCacheProvider
    {
        private ConcurrentDictionary<string, DefaultMemoryCacheItem> _Memory = new ConcurrentDictionary<string, DefaultMemoryCacheItem>();

        private readonly ILogger _Logger;

        public DefaultMemoryCacheProvider(IFileLogger fileLogger, IZooKeeper zooKeeper)
        {
            _Logger = fileLogger;
            Persistent = true;
            SecondsToClean = 60;
            zooKeeper.Add(this);
        }

        private bool _Alive = false;

        private object _Locker = new object();

        public bool Persistent { get; set; }

        public int SecondsToClean { get; set; }

        private void Startup()
        {
            if (_Alive)
            {
                return;
            }

            lock (_Locker)
            {
                if (!_Alive)
                {
                    if (Persistent)
                    {
                        try
                        {
                            var items = new DefaultMemoryCachePersistence().RestoreFromFile();
                            if (items != null)
                            {
                                foreach (var item in items)
                                {
                                    _Memory.TryAdd(item.Key, item);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Error, "Default Memory Cache Provider persistence failed.", e);
                        }
                    }

                    _Alive = true;

                    new Thread(() =>
                    {
                        _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Information, "Default Memory Cache Provider started.");
                        
                        try
                        {
                            while (_Alive)
                            {
                                Thread.Sleep(1000 * SecondsToClean);

                                var items = _Memory.ToArray();
                                foreach (var item in items)
                                {
                                    if (DateTime.Now > item.Value.ExpiryTime)
                                    {
                                        _Memory.TryRemove(item.Key, out var i);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Error, e);
                        }
                        finally
                        {
                            _Alive = false;
                        }
                    })
                    {
                        IsBackground = true,
                        Name = "MemoryCacheProvider",
                    }.Start();
                }
            }
        }

        public T Get<T>(string key)
        {
            if (!_Alive)
            {
                Startup();
            }

            if (_Memory.TryGetValue(key.Md5(), out var item))
            {
                if (DateTime.Now < item.ExpiryTime)
                {
                    return (T)item.Value;
                }
                else
                {
                    Remove(key.Md5());
                }
            }

            return default(T);
        }

        public Task<T> GetAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T GetOrSet<T>(string key, SetDelegate<T> setDelegate, TimeSpan expiry)
        {
            if (!_Alive)
            {
                Startup();
            }

            if (_Memory.TryGetValue(key.Md5(), out var item))
            {
                if (DateTime.Now < item.ExpiryTime)
                {
                    return (T)item.Value;
                }
                else
                {
                    var addItem = new DefaultMemoryCacheItem()
                    {
                        Key = key.Md5(),
                        Value = setDelegate(this),
                        ExpiryTime = DateTime.Now.Add(expiry),
                    };
                    try
                    {
                        return (T)_Memory.AddOrUpdate(key, addItem, (x, y) => addItem).Value;
                    }
                    catch (Exception e)
                    {
                        _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Error, e);
                    }

                    return (T)item.Value;
                }
            }
            else
            {
                return (T)_Memory.GetOrAdd(key.Md5(), new DefaultMemoryCacheItem()
                {
                    Key = key.Md5(),
                    Value = setDelegate(this),
                    ExpiryTime = DateTime.Now.Add(expiry),
                }).Value;
            }
        }

        public Task<T> GetOrSetAsync<T>(string key, SetAsyncDelegate<T> setDelegate, TimeSpan expiry)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            if (!_Alive)
            {
                Startup();
            }

            return _Memory.TryRemove(key.Md5(), out var value);
        }

        public Task<bool> RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T value, TimeSpan expiry)
        {
            if (!_Alive)
            {
                Startup();
            }

            var addItem = new DefaultMemoryCacheItem()
            {
                Key = key.Md5(),
                Value = value,
                ExpiryTime = DateTime.Now.Add(expiry),
            };

            try
            {
                _Memory.AddOrUpdate(key.Md5(), addItem, (x, y) => addItem);
                return true;
            }
            catch(Exception e)
            {
                _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Error, e);
            }

            return false;
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _Alive = false;

            _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Information, "Default Memory Cache Provider is disposing.");
            Console.WriteLine("Default Memory Cache Provider is disposing.");

            if (Persistent)
            {
                try
                {
                    new DefaultMemoryCachePersistence().StoreToFile(_Memory.Values);
                }
                catch (Exception e)
                {
                    _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Error, "Default Memory Cache Provider persistence failed.", e);
                }
            }

            _Logger.LogEvent(nameof(DefaultMemoryCacheProvider), Severity.Information, "Default Memory Cache Provider is disposed.");
            Console.WriteLine("Default Memory Cache Provider is disposed.");
        }
    }
}
