using Guru.AspNetCore.Attributes;
using Guru.Formatter.Json;

namespace Guru.Restful
{
    public abstract class RequestBase
    {
        [JsonProperty("head")]
        [Annotation(Hidden = true)]
        public RequestHead Head { get; set; }
    }
}