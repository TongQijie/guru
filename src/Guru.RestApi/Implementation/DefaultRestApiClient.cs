using System;
using Guru.ExtensionMethod;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Abstractions;
using Guru.Logging;
using Guru.Logging.Abstractions;
using Guru.Network;
using Guru.Network.Abstractions;
using Guru.RestApi.Abstractions;
using System.IO;
using System.Text;

namespace Guru.RestApi.Implementation
{
    [Injectable(typeof(IRestApiClient), Lifetime.Singleton)]
    internal class DefaultRestApiClient : IRestApiClient
    {
        public string BaseUrl { get; set; }

        private readonly IHttpClientRequest _HttpClientRequest;

        private readonly ILogger _Logger;

        private readonly ILightningFormatter _Formatter;

        private string _Auth = null;

        public string Auth
        {
            get
            {
                if (!_Auth.HasValue() && "./auth".IsFile())
                {
                    var data = new byte[1024];
                    var count = 0;
                    using (var inputStream = new FileStream("./auth".FullPath(), FileMode.Open, FileAccess.Read))
                    {
                        count = inputStream.Read(data, 0, data.Length);
                    }
                    _Auth = Encoding.UTF8.GetString(data, 0, count);
                }

                return _Auth;
            }
            set
            {
                var data = Encoding.UTF8.GetBytes(value);
                using (var outputStream = new FileStream("./auth".FullPath(), FileMode.Create, FileAccess.Write))
                {
                    outputStream.Write(data, 0, data.Length);
                }

                _Auth = value;
            }
        }

        public DefaultRestApiClient(IHttpClientBroker httpClientBroker, IFileLogger fileLogger, IJsonLightningFormatter formatter)
        {
            _HttpClientRequest = httpClientBroker.Get(new DefaultHttpClientSettings("DefaultRestApiClient"));
            _Logger = fileLogger;
            _Formatter = formatter;
        }

        public async Task<TResponse> Request<TRequest, TResponse>(TRequest request, string serviceName, string methodName)
            where TRequest: IAuthRestApiRequest
            where TResponse: IRestApiResponse 
        {
            if (request.Head == null)
            {
                request.Head = new AuthRestApiRequestHead();
            }
            request.Head.Auth = Auth;

            try
            {
                using (var response = await _HttpClientRequest.PostAsync($"{BaseUrl}/{serviceName}/{methodName}", null, request, _Formatter, null))
                {
                    if (response.StatusCode == 200)
                    {
                        return await response.GetBodyAsync<TResponse>(_Formatter);
                    }
                    else
                    {
                        _Logger.LogEvent(nameof(DefaultRestApiClient), Severity.Error, $"{BaseUrl}/{serviceName}/{methodName} : {response.StatusCode}");
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.LogEvent(nameof(DefaultRestApiClient), Severity.Error, e);
            }

            return default(TResponse);
        }
    }
}