using Guru.AspNetCore.Attributes;
using Guru.Cache.Abstractions;
using System;

namespace ClassLib
{
    [Api("testApi")]
    public class TestApi
    {
        private readonly IMemoryCacheProvider _MemoryCacheProvider;

        public TestApi(IMemoryCacheProvider memoryCacheProvider)
        {
            _MemoryCacheProvider = memoryCacheProvider;
        }

        [ApiMethod("sayHi")]
        public string SayHi(string word)
        {
            return word;
        }

        [ApiMethod("set")]
        public void Set(string key)
        {
            _MemoryCacheProvider.Set(key, new CacheObject() { Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, TimeSpan.FromSeconds(300));
        }

        [ApiMethod("get")]
        public CacheObject Get(string key)
        {
            return _MemoryCacheProvider.Get<CacheObject>(key);
        }

        public class CacheObject
        {
            public string Value { get; set; }
        }
    }
}