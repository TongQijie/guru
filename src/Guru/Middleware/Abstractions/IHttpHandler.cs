using System.Threading.Tasks;

namespace Guru.Middleware.Abstractions
{
    public interface IHttpHandler
    {
         Task ProcessRequest(ICallingContext context);
    }
}