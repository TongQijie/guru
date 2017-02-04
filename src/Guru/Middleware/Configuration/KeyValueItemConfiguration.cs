using Guru.Formatter.Json;

namespace Guru.Middleware.Configuration
{
    public class KeyValueItemConfiguration
    {
        [JsonProperty(Alias = "key")]
        public string Key { get; set; }

        [JsonProperty(Alias = "value")]
        public string Value { get; set; }
    }
}