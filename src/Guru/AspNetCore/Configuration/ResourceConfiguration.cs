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
        }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("dir")]
        public string Directory { get; set; }

        [JsonProperty("mineTypes")]
        public Dictionary<string, string> MineTypes { get; set; }
    }
}