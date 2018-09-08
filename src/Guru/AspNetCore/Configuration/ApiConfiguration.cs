using Guru.Formatter.Json;

namespace Guru.AspNetCore.Configuration
{
    public class ApiConfiguration
    {
        public ApiConfiguration()
        {
            Prefix = "api";
        }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("enableLog")]
        public bool EnableLog { get; set; }

        [JsonProperty("enableMetadata")]
        public bool EnableMetadata { get; set; }
        
        [JsonProperty("headers")]
        public HeaderConfiguration[] Headers { get; set; }
    }
}