using Guru.Formatter.Json;

namespace Guru.AspNetCore.Configuration
{
    public class RouterConfiguration
    {
        [JsonProperty(Alias = "default")]
        public string Default { get; set; }

        [JsonProperty(Alias = "rewrites")]
        public RewriteRuleConfiguration[] RewriteRules { get; set; }
    }
}