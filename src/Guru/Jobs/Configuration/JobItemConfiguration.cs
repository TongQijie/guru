using Guru.Formatter.Json;

namespace Guru.Jobs.Configuration
{
    public class JobItemConfiguration
    {
        [JsonProperty(Alias = "name")]
        public string Name { get; set; }

        [JsonProperty(Alias = "type")]
        public string Type { get; set; }

        [JsonProperty(Alias = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(Alias = "args")]
        public string[] Args { get; set; }

        [JsonProperty(Alias = "schedule")]
        public JobScheduleConfiguration Schedule { get; set; }
    }
}