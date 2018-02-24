using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Json;

namespace Guru.AspNetCore.Configuration
{
    [StaticFile(typeof(IApplicationConfiguration), "./Configuration/app.json")]
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            Resource = new ResourceConfiguration();
            Api = new ApiConfiguration();
        }

        [JsonProperty(Alias = "appId")]
        public string AppId { get; set; }

        [JsonProperty(Alias = "urls")]
        public string[] Urls { get; set; }

        [JsonProperty(Alias = "router")]
        public RouterConfiguration Router { get; set; }

        [JsonProperty(Alias = "res")]
        public ResourceConfiguration Resource { get; set; }

        [JsonProperty(Alias = "api")]
        public ApiConfiguration Api { get; set; }
    }
}