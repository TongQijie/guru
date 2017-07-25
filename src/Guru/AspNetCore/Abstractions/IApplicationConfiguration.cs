using Guru.AspNetCore.Configuration;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApplicationConfiguration
    {
        RouterConfiguration Router { get; set; }

        ResourceConfiguration Resource { get; set; }

        ApiConfiguration Api { get; set; }
    }
}