using Guru.Cache.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.RestApi.Abstractions;
using Guru.ExtensionMethod;
using System.Collections.Generic;
using System;

namespace Guru.RestApi.Implementation
{
    [Injectable(typeof(IAuthManager), Lifetime.Singleton)]
    public class DefaultAuthManager : IAuthManager
    {
        private readonly ICacheProvider _CacheProvider;

        public DefaultAuthManager()
        {
            _CacheProvider = DependencyContainer.Resolve<ICacheProvider>("DefaultCache");
        }

        public void Create(string auth, string uid)
        {
            _CacheProvider.Set(auth, uid, TimeSpan.FromDays(3));
        }

        public bool Validate(AuthRestApiRequestHead head)
        {
            if (head == null || !head.Auth.HasValue())
            {
                return false;
            }

            var uid = _CacheProvider.Get<string>(head.Auth);
            if (!uid.HasValue())
            {
                return false;
            }

            if (head.Extensions == null)
            {
                head.Extensions = new Dictionary<string, string>();
            }

            head.Extensions["uid"] = uid;
            return true;
        }
    }
}