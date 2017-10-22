using System.Threading.Tasks;

namespace Guru.AspNetCore.Abstractions
{
    public interface IAspNetCoreComponent
    {
        bool NeedLog { get; set; }

        Task Process(CallingContext context);
    }
}