using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Abstractions;
using Guru.Logging.Abstractions;
using Guru.Network;
using Guru.Network.Abstractions;
using Guru.RestApi.Abstractions;

namespace Guru.RestApi.Implementation
{
    [Injectable(typeof(IRestApiClient), Lifetime.Singleton)]
    internal class DefaultRestApiClient : IRestApiClient
    {
        public string BaseUrl { get; set; }

        private readonly IHttpClientRequest _HttpClientRequest;

        private readonly ILogger _Logger;

        public DefaultRestApiClient(IHttpClientBroker httpClientBroker, IFileLogger fileLogger)
        {
            _HttpClientRequest = httpClientBroker.Get(new DefaultHttpClientSettings("DefaultRestApiClient"));
            _Logger = fileLogger;
        }

        public async Task<TResponse> Request<TRequest, TResponse>(TRequest request, string serviceName, string methodName)
        {
            try
            {
                using (var response = await _HttpClientRequest.PostAsync<IJsonLightningFormatter>($"{BaseUrl}/{serviceName}/{methodName}", request, new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json" }
                }))
                {
                    return await response.GetBodyAsync<TResponse, IJsonLightningFormatter>();
                }
            }
            catch (Exception e)
            {
                _Logger.LogEvent(nameof(DefaultRestApiClient), Logging.Severity.Error, e);
            }

            return default(TResponse);
        }
    }
}