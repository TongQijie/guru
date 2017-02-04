using System.Threading.Tasks;

namespace Guru.Middleware.Abstractions
{
    public interface IStaticFileHandler
    {
        Task ProcessRequest(ICallingContext context);
    }
}