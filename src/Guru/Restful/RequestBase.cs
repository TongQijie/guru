using Guru.Formatter.Json;

namespace Guru.Restful
{
    public abstract class RequestBase
    {
        [JsonProperty("head")]
        public RequestHead Head { get; set; }
    }
}