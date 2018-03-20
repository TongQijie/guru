using Guru.Cache.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.RestApi.Abstractions;
using Guru.ExtensionMethod;
using System;

namespace Guru.RestApi.Implementation
{
    [Injectable(typeof(IAuthManager), Lifetime.Singleton)]
    public class DefaultAuthManager : IAuthManager
    {
        private readonly ICacheProvider _CacheProvider;

        public DefaultAuthManager(IMemoryCacheProvider memoryCacheProvider)
        {
            _CacheProvider = DependencyContainer.ResolveOrDefault<ICacheProvider>("DefaultCache");
        }

        public void Create(string auth, string uid)
        {
            _CacheProvider.Set(auth, uid, TimeSpan.FromDays(3));
        }

        public bool Validate(IAuthRestApiRequest request)
        {
            var auth = request.GetAuth();
            if (!auth.HasValue())
            {
                return false;
            }

            var uid = _CacheProvider.Get<string>(auth);
            if (!uid.HasValue())
            {
                return false;
            }

            request.SetUid(uid);
            return true;
        }
    }
}