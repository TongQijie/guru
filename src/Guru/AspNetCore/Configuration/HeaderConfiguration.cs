using Guru.Formatter.Json;

namespace Guru.AspNetCore.Configuration
{
    public class HeaderConfiguration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("values")]
        public string[] Values { get; set; }
    }
}
