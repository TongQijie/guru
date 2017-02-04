using Guru.Formatter.Json;

namespace Guru.Middleware.Configuration
{
    public class StaticResouceConfiguration
    {
        [JsonProperty(Alias = "type")]
        public string ResourceType { get; set; }

        [JsonProperty(Alias = "path")]
        public string AllowPath { get; set; }

        [JsonProperty(Alias = "content")]
        public string ContentType { get; set; }
    }
}