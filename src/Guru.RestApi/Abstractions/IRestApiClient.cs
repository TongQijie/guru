using System.Threading.Tasks;

namespace Guru.RestApi.Abstractions
{
    public interface IRestApiClient
    {
        Task<TResponse> Request<TRequest, TResponse>(TRequest request, string serviceName, string methodName);
    }
}