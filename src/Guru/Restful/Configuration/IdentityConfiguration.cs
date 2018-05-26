using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Json;
using Guru.Restful.Abstractions;

namespace Guru.Restful.Configuration
{
    [StaticFile(typeof(IIdentityConfiguration), "./Configuration/identity.json")]
    public class IdentityConfiguration : IIdentityConfiguration
    {
        [JsonProperty("expire")]
        public long ExpireMillis { get; set; }

        [JsonProperty("renew")]
        public long RenewMillis { get; set; }
    }
}