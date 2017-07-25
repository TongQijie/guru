using Guru.Formatter.Json;

namespace Guru.AspNetCore.Configuration
{
    public class RewriteRuleConfiguration
    {
        [JsonProperty(Alias = "pattern")]
        public string Pattern { get; set; }

        [JsonProperty(Alias = "value")]
        public string Value { get; set; }

        [JsonProperty(Alias = "mode")]
        public RewriteMode Mode { get; set; }
    }
}