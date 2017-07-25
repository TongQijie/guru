using System.Threading.Tasks;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiProvider
    {
         Task<IApiContext> GetApi(CallingContext context);
    }
}