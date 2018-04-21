using System.Threading.Tasks;

namespace Guru.RestApi.Abstractions
{
    public interface IRestApiClient
    {
        string BaseUrl { get; set; }

        string Auth { get; set; }

        Task<TResponse> Request<TRequest, TResponse>(TRequest request, string serviceName, string methodName)
            where TRequest : IAuthRestApiRequest
            where TResponse : IRestApiResponse;
    }
}