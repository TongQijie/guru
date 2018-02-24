using Guru.AspNetCore.Configuration;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApplicationConfiguration
    {
        string AppId { get; set; }

        string[] Urls { get; set; }

        RouterConfiguration Router { get; set; }

        ResourceConfiguration Resource { get; set; }

        ApiConfiguration Api { get; set; }
    }
}