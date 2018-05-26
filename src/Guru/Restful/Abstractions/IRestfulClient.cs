using System;
using System.Net;
using System.Threading.Tasks;

namespace Guru.Restful.Abstractions
{
    public interface IRestfulClient
    {
        string BaseUrl { get; set; }

        string Token { get; set; }

        void Configure(IWebProxy webProxy, TimeSpan? timeout);

        Task<TResponse> Request<TRequest, TResponse>(TRequest request, string serviceName, string methodName)
            where TRequest : RequestBase
            where TResponse : ResponseBase;
    }
}