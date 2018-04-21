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

        public DefaultAuthManager()
        {
            _CacheProvider = DependencyContainer.ResolveOrDefault<ICacheProvider, IMemoryCacheProvider>("DefaultCache");
        }

        public string New(string uid, TimeSpan expiryTimeSpan)
        {
            var auth = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            _CacheProvider.Set(auth, new CacheEntity()
            {
                Uid = uid,
                Deadline = DateTime.Now.Add(expiryTimeSpan),
                Milliseconds = (long)expiryTimeSpan.TotalMilliseconds,
            }, expiryTimeSpan);
            return auth;
        }

        public bool Validate(IAuthRestApiRequest request)
        {
            var auth = request.GetAuth();
            if (!auth.HasValue())
            {
                return false;
            }

            var entity = _CacheProvider.Get<CacheEntity>(auth);
            if (entity == null)
            {
                return false;
            }

            if ((entity.Deadline - DateTime.Now) < TimeSpan.FromMinutes(10))
            {
                _CacheProvider.Set(auth, new CacheEntity()
                {
                    Uid = entity.Uid,
                    Deadline = DateTime.Now.AddMilliseconds(entity.Milliseconds),
                    Milliseconds = entity.Milliseconds,
                }, TimeSpan.FromMilliseconds(entity.Milliseconds));
            }

            request.SetUid(entity.Uid);
            return true;
        }

        public class CacheEntity
        {
            public string Uid { get; set; }

            public DateTime Deadline { get; set; }

            public long Milliseconds { get; set; }
        }
    }
}