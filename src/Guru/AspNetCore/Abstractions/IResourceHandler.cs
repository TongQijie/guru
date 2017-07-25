using System.Threading.Tasks;

namespace Guru.AspNetCore.Abstractions
{
    public interface IResourceHandler
    {
         Task ProcessRequest(CallingContext context);
    }
}