using System;
using System.Threading.Tasks;
using Guru.Cache.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Restful.Abstractions;

namespace Guru.Restful.Implementation
{
    [Injectable(typeof(IIdentityValidator), Lifetime.Singleton)]
    internal class DefaultIdentityValidator : IIdentityValidator
    {
        private readonly ICacheProvider _CacheProvider;

        public DefaultIdentityValidator()
        {
            _CacheProvider = DependencyContainer.ResolveOrDefault<ICacheProvider, IMemoryCacheProvider>("DefaultCache");
        }

        public string Create(string userId)
        {
            var token = GenerateToken();

            var config = DependencyContainer.Resolve<IIdentityConfiguration>();

            var expire = config.ExpireMillis <= 0 ? (long)TimeSpan.FromDays(30).TotalMilliseconds : config.ExpireMillis;

            _CacheProvider.Set(token, new CacheEntity()
            {
                UserId = userId,
                Deadline = DateTime.Now.AddMilliseconds(expire),
                Milliseconds = expire,
            }, TimeSpan.FromMilliseconds(expire));

            return token;
        }

        public async Task<string> CreateAsync(string userId)
        {
            var token = GenerateToken();

            var config = DependencyContainer.Resolve<IIdentityConfiguration>();

            var expire = config.ExpireMillis <= 0 ? (long)TimeSpan.FromDays(30).TotalMilliseconds : config.ExpireMillis;

            await _CacheProvider.SetAsync(token, new CacheEntity()
            {
                UserId = userId,
                Deadline = DateTime.Now.AddMilliseconds(expire),
                Milliseconds = expire,
            }, TimeSpan.FromMilliseconds(expire));

            return token;
        }

        public bool Validate(RequestHead head)
        {
            if (head == null || !head.Token.HasValue())
            {
                return false;
            }

            var entity = _CacheProvider.Get<CacheEntity>(head.Token);
            if (entity == null)
            {
                return false;
            }

            var config = DependencyContainer.Resolve<IIdentityConfiguration>();

            var renew = config.RenewMillis <= 0 ? (long)TimeSpan.FromDays(1).TotalMilliseconds : config.RenewMillis;

            if ((entity.Deadline - DateTime.Now) < TimeSpan.FromMilliseconds(renew))
            {
                var expire = config.ExpireMillis <= 0 ? (long)TimeSpan.FromDays(30).TotalMilliseconds : config.ExpireMillis;

                _CacheProvider.Set(head.Token, new CacheEntity()
                {
                    UserId = entity.UserId,
                    Deadline = DateTime.Now.AddMilliseconds(expire),
                    Milliseconds = expire,
                }, TimeSpan.FromMilliseconds(expire));
            }

            head.SetUserId(entity.UserId);
            return true;
        }

        public async Task<bool> ValidateAsync(RequestHead head)
        {
            if (head == null || !head.Token.HasValue())
            {
                return false;
            }

            var entity = await _CacheProvider.GetAsync<CacheEntity>(head.Token);
            if (entity == null)
            {
                return false;
            }

            var config = DependencyContainer.Resolve<IIdentityConfiguration>();

            var renew = config.RenewMillis <= 0 ? (long)TimeSpan.FromDays(1).TotalMilliseconds : config.RenewMillis;

            if ((entity.Deadline - DateTime.Now) < TimeSpan.FromMilliseconds(renew))
            {
                var expire = config.ExpireMillis <= 0 ? (long)TimeSpan.FromDays(30).TotalMilliseconds : config.ExpireMillis;

                await _CacheProvider.SetAsync(head.Token, new CacheEntity()
                {
                    UserId = entity.UserId,
                    Deadline = DateTime.Now.AddMilliseconds(expire),
                    Milliseconds = expire,
                }, TimeSpan.FromMilliseconds(expire));
            }

            head.SetUserId(entity.UserId);
            return true;
        }

        public class CacheEntity
        {
            public string UserId { get; set; }

            public DateTime Deadline { get; set; }

            public long Milliseconds { get; set; }
        }

        private string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "_").Replace("+", "-");
        }
    }
}