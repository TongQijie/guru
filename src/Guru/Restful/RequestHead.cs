using System.Collections.Generic;
using Guru.Formatter.Json;

namespace Guru.Restful
{
    public class RequestHead
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("extensions")]
        public Dictionary<string, string> Extensions { get; set; }
    }
}