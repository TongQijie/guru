using Guru.Formatter.Json;
using Guru.DependencyInjection;

namespace Guru.Jobs.Configuration
{
    [FileDI(typeof(IApplicationConfiguration), "./configuration/app.json", Format = FileFormat.Json)]
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        [JsonProperty(Alias = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(Alias = "jobs")]
        public JobItemConfiguration[] Jobs { get; set; }
    }
}