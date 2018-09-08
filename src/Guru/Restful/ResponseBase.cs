using Guru.AspNetCore.Attributes;
using Guru.Formatter.Json;

namespace Guru.Restful
{
    public abstract class ResponseBase
    {
        [JsonProperty("head")]
        [Annotation(Hidden = true)]
        public ResponseHead Head { get; set; }
    }
}