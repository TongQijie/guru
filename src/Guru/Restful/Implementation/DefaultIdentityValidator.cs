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

        private readonly IIdentityTokenPersistence _IdentityTokenPersistence;

        public DefaultIdentityValidator()
        {
            _CacheProvider = DependencyContainer.ResolveOrDefault<ICacheProvider, IMemoryCacheProvider>("DefaultCache");
            _IdentityTokenPersistence = DependencyContainer.Resolve<IIdentityTokenPersistence>();
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
            }, TimeSpan.FromMilliseconds(expire));

            if (_IdentityTokenPersistence != null)
            {
                _IdentityTokenPersistence.Set(new IdentityToken()
                {
                    UserId = userId,
                    Token = token,
                    Deadline = DateTime.Now.AddMilliseconds(expire),
                });
            }

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
            }, TimeSpan.FromMilliseconds(expire));

            if (_IdentityTokenPersistence != null)
            {
                await _IdentityTokenPersistence.SetAsync(new IdentityToken()
                {
                    UserId = userId,
                    Token = token,
                    Deadline = DateTime.Now.AddMilliseconds(expire),
                });
            }

            return token;
        }

        public bool Validate(RequestHead head)
        {
            if (head == null || !head.Token.HasValue())
            {
                return false;
            }

            var config = DependencyContainer.Resolve<IIdentityConfiguration>();
            var expire = config.ExpireMillis <= 0 ? (long)TimeSpan.FromDays(30).TotalMilliseconds : config.ExpireMillis;

            var entity = _CacheProvider.Get<CacheEntity>(head.Token);

            if (entity == null)
            {
                if (_IdentityTokenPersistence != null)
                {
                    var identityToken = _IdentityTokenPersistence.Get(head.Token);
                    if (identityToken != null)
                    {
                        entity = new CacheEntity()
                        {
                            Deadline = identityToken.Deadline,
                            UserId = identityToken.UserId,
                        };

                        _CacheProvider.Set(head.Token, entity, TimeSpan.FromMilliseconds(expire));
                    }
                }
            }

            if (entity == null)
            {
                return false;
            }

            var renew = config.RenewMillis <= 0 ? (long)TimeSpan.FromDays(1).TotalMilliseconds : config.RenewMillis;

            if ((entity.Deadline - DateTime.Now) < TimeSpan.FromMilliseconds(renew))
            {
                _CacheProvider.Set(head.Token, new CacheEntity()
                {
                    UserId = entity.UserId,
                    Deadline = DateTime.Now.AddMilliseconds(expire),
                }, TimeSpan.FromMilliseconds(expire));

                if (_IdentityTokenPersistence != null)
                {
                    _IdentityTokenPersistence.Set(new IdentityToken()
                    {
                        UserId = entity.UserId,
                        Token = head.Token,
                        Deadline = DateTime.Now.AddMilliseconds(expire),
                    });
                }
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

            var config = DependencyContainer.Resolve<IIdentityConfiguration>();
            var expire = config.ExpireMillis <= 0 ? (long)TimeSpan.FromDays(30).TotalMilliseconds : config.ExpireMillis;

            var entity = await _CacheProvider.GetAsync<CacheEntity>(head.Token);
            if (entity == null)
            {
                if (_IdentityTokenPersistence != null)
                {
                    var identityToken = await _IdentityTokenPersistence.GetAsync(head.Token);
                    if (identityToken != null)
                    {
                        entity = new CacheEntity()
                        {
                            Deadline = identityToken.Deadline,
                            UserId = identityToken.UserId,
                        };

                        await _CacheProvider.SetAsync(head.Token, entity, TimeSpan.FromMilliseconds(expire));
                    }
                }
            }

            if (entity == null)
            {
                return false;
            }

            var renew = config.RenewMillis <= 0 ? (long)TimeSpan.FromDays(1).TotalMilliseconds : config.RenewMillis;

            if ((entity.Deadline - DateTime.Now) < TimeSpan.FromMilliseconds(renew))
            {
                await _CacheProvider.SetAsync(head.Token, new CacheEntity()
                {
                    UserId = entity.UserId,
                    Deadline = DateTime.Now.AddMilliseconds(expire),
                }, TimeSpan.FromMilliseconds(expire));

                if (_IdentityTokenPersistence != null)
                {
                    await _IdentityTokenPersistence.SetAsync(new IdentityToken()
                    {
                        UserId = entity.UserId,
                        Token = head.Token,
                        Deadline = DateTime.Now.AddMilliseconds(expire),
                    });
                }
            }

            head.SetUserId(entity.UserId);
            return true;
        }

        public class CacheEntity
        {
            public string UserId { get; set; }

            public DateTime Deadline { get; set; }
        }

        private string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "_").Replace("+", "-");
        }
    }
}