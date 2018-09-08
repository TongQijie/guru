using System.Threading.Tasks;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiMetadataHandler
    {
        Task ProcessRequest(CallingContext context);
    }
}