using Guru.Auth.Abstractions;
using Guru.Cache.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using System;

namespace Guru.Auth.Implementation
{
    [Injectable(typeof(IAuthValidator), Lifetime.Singleton)]
    public class AuthValidator : IAuthValidator
    {
        private readonly ICacheProvider _CacheProvider;

        public AuthValidator()
        {
            _CacheProvider = DependencyContainer.Resolve<ICacheProvider>("DefaultCache");
        }

        public void AddUid(string auth, string uid)
        {
            _CacheProvider.Set(auth, uid, TimeSpan.FromDays(3));
        }

        public void Validate(IAuthRequest authRequest)
        {
            if (authRequest == null || authRequest.Head == null || string.IsNullOrEmpty(authRequest.Head.Auth))
            {
                return;
            }

            authRequest.Head.Uid = _CacheProvider.Get<string>(authRequest.Head.Auth);
        }
    }
}