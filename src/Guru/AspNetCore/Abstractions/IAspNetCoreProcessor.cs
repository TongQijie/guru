using System.Threading.Tasks;

namespace Guru.AspNetCore.Abstractions
{
    public interface IAspNetCoreProcessor
    {
         Task Process(CallingContext context);
    }
}