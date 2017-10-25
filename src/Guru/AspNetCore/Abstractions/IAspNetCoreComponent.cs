using System.Threading.Tasks;

namespace Guru.AspNetCore.Abstractions
{
    public interface IAspNetCoreComponent
    {
        Task Process(CallingContext context);
    }
}