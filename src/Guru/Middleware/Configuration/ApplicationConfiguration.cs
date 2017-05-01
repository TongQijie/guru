using Guru.Formatter.Json;
using Guru.DependencyInjection;

namespace Guru.Middleware.Configuration
{
    [FileDI(typeof(IApplicationConfiguration), "./Configuration/app.json", Format = FileFormat.Json)]
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        [JsonProperty(Alias = "routes")]
        public KeyValueItemConfiguration[] Routes { get; set; }

        [JsonProperty(Alias = "headers")]
        public KeyValueItemConfiguration[] Headers { get; set; }

        [JsonProperty(Alias = "resources")]
        public StaticResouceConfiguration[] Resources { get; set; }

        [JsonProperty(Alias = "rewrites")]
        public RewriteConfiguration[] Rewrites { get; set; }
    }
}