using System;
using System.Text;
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

            _CacheProvider.Set(token, new CacheEntity()
            {
                UserId = userId,
                Deadline = DateTime.Now.AddDays(30),
                Milliseconds = (long)TimeSpan.FromDays(30).TotalMilliseconds,
            }, TimeSpan.FromDays(30));

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

            if ((entity.Deadline - DateTime.Now) < TimeSpan.FromDays(1))
            {
                _CacheProvider.Set(head.Token, new CacheEntity()
                {
                    UserId = entity.UserId,
                    Deadline = DateTime.Now.AddMilliseconds(entity.Milliseconds),
                    Milliseconds = entity.Milliseconds,
                }, TimeSpan.FromMilliseconds(entity.Milliseconds));
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