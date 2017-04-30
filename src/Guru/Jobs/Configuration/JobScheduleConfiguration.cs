using Guru.Formatter.Json;
using System;

namespace Guru.Jobs.Configuration
{
    public class JobScheduleConfiguration
    {
        [JsonProperty(Alias = "startTime")]
        public DateTime? StartTime { get; set; }

        [JsonProperty(Alias = "endTime")]
        public DateTime? EndTime { get; set; }

        [JsonProperty(Alias = "cycle")]
        public ExecutionCycle Cycle { get; set; }

        [JsonProperty(Alias = "year")]
        public int Year { get; set; }

        [JsonProperty(Alias = "month")]
        public int Month { get; set; }

        [JsonProperty(Alias = "day")]
        public int Day { get; set; }

        [JsonProperty(Alias = "hour")]
        public int Hour { get; set; }

        [JsonProperty(Alias = "minute")]
        public int Minute { get; set; }

        [JsonProperty(Alias = "second")]
        public int Second { get; set; }
    }
}