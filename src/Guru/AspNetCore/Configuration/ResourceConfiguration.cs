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

        public string Prefix { get; set; }

        public string Directory { get; set; }

        public Dictionary<string, string> MineTypes { get; set; }
    }
}