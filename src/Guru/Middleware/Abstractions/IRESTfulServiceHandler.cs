using System.Threading.Tasks;

namespace Guru.Middleware.Abstractions
{
    public interface IRESTfulServiceHandler
    {
        Task ProcessRequest(ICallingContext context);
    }
}