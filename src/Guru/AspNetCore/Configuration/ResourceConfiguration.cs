using Guru.Formatter.Json;
using System.Collections.Generic;

namespace Guru.AspNetCore.Configuration
{
    public class ResourceConfiguration
    {
        public ResourceConfiguration()
        {
            Prefix = "res";
            Directory = "./wwwroot";
            MaxRangeBytes = 2 * 1024 * 1024; // default 2Mb
        }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("dir")]
        public string Directory { get; set; }

        [JsonProperty("mineTypes")]
        public Dictionary<string, string> MineTypes { get; set; }

        [JsonProperty("maxRangeBytes")]
        public long MaxRangeBytes { get; set; }

        [JsonProperty("enableLog")]
        public bool EnableLog { get; set; }
    }
}