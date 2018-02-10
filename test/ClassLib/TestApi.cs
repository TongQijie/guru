using Guru.AspNetCore.Attributes;
using Guru.Cache.Abstractions;
using Guru.RestApi;
using Guru.RestApi.Abstractions;
using System;

namespace ClassLib
{
    [Api("testApi")]
    public class TestApi
    {
        private readonly IMemoryCacheProvider _MemoryCacheProvider;

        private static IAuthManager _AuthManager;

        public TestApi(IMemoryCacheProvider memoryCacheProvider, IAuthManager authManager)
        {
            _MemoryCacheProvider = memoryCacheProvider;
            _AuthManager = authManager;
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
            _AuthManager.Create(auth, uid);
            return auth;
        }

        [ApiMethod("query")]
        [RestApiPrefix(AuthValidatingEnum.Required)]
        public QueryResponse Query(QueryRequest request)
        {
            return new QueryResponse()
            {
                Uid = request.Head.Extensions["uid"],
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

        public class QueryRequest : IAuthRestApiRequest
        {
            public AuthRestApiRequestHead Head { get; set; }
        }

        public class QueryResponse : IRestApiResponse
        {
            public RestApiResponseHead Head { get; set; }

            public string Uid { get; set; }
        }
    }
}