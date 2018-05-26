using Guru.Formatter.Json;

namespace Guru.Restful
{
    public abstract class ResponseBase
    {
        [JsonProperty("head")]
        public ResponseHead Head { get; set; }
    }
}