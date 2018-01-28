using Guru.AspNetCore.Attributes;
using Guru.Auth;
using Guru.Auth.Abstractions;
using Guru.Cache.Abstractions;
using System;

namespace ClassLib
{
    [Api("testApi")]
    public class TestApi
    {
        private readonly IMemoryCacheProvider _MemoryCacheProvider;

        private static IAuthValidator _AuthValidator;

        public TestApi(IMemoryCacheProvider memoryCacheProvider, IAuthValidator authValidator)
        {
            _MemoryCacheProvider = memoryCacheProvider;
            _AuthValidator = authValidator;
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

        [ApiMethod("login")]
        public string Login(string uid)
        {
            var auth = Guid.NewGuid().ToString().Replace("-", "");
            _AuthValidator.AddUid(auth, uid);
            return auth;
        }

        [ApiMethod("query")]
        [AuthValidate]
        public QueryResponse Query(QueryRequest request)
        {
            return new QueryResponse()
            {
                Uid = request.Head.Uid,
            };
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

        public class QueryRequest : IAuthRequest
        {
            public AuthRequestHeader Head { get; set; }
        }

        public class QueryResponse : IAuthResponse
        {
            public AuthResponseHeader Head { get; set; }

            public string Uid { get; set; }
        }
    }
}