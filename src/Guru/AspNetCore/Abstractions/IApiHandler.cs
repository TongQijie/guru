using System.Threading.Tasks;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiHandler
    {
        Task ProcessRequest(CallingContext context);
    }
}