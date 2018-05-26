using Guru.Formatter.Json;

namespace Guru.Restful
{
    public class ResponseHead
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}