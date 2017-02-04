using Guru.Formatter.Json;

namespace Guru.Middleware.Configuration
{
    public class RewriteConfiguration
    {
        [JsonProperty(Alias = "pattern")]
        public string Pattern { get; set; }

        [JsonProperty(Alias = "value")]
        public string Value { get; set; }

        [JsonProperty(Alias = "mode")]
        public RewriteMode Mode { get; set; }
    }
}