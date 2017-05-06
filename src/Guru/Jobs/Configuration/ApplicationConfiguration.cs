using Guru.Formatter.Json;
using Guru.DependencyInjection.Attributes;

namespace Guru.Jobs.Configuration
{
    [StaticFile(typeof(IApplicationConfiguration), "./Configuration/app.json")]
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        [JsonProperty(Alias = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(Alias = "jobs")]
        public JobItemConfiguration[] Jobs { get; set; }
    }
}