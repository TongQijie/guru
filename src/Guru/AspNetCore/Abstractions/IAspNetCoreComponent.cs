using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Guru.AspNetCore.Abstractions
{
    public interface IAspNetCoreComponent
    {
         Task Process(CallingContext context);
    }
}