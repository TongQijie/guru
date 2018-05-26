using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Abstractions;
using Guru.Logging;
using Guru.Logging.Abstractions;
using Guru.Network.Abstractions;
using Guru.Restful.Abstractions;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Guru.Restful.Implementation
{
    [Injectable(typeof(IRestfulClient), Lifetime.Transient)]
    internal class DefaultRestfulClient : IRestfulClient
    {
        private readonly IHttpManager _HttpManager;

        private readonly ILightningFormatter _Formatter;

        private readonly ILogger _Logger;

        private IHttpRequest _HttpRequest;

        public DefaultRestfulClient(IHttpManager httpManager, IFileLogger logger, IJsonLightningFormatter formatter)
        {
            formatter.OmitNullValue = true;
            _HttpManager = httpManager;
            _Formatter = formatter;
            _Logger = logger;
            _HttpRequest = httpManager.Create();
        }

        public void Configure(IWebProxy webProxy, TimeSpan? timeout)
        {
            _HttpRequest = _HttpManager.Create(webProxy, timeout);
        }

        public string BaseUrl { get; set; }

        public string Token { get; set; }

        public async Task<TResponse> Request<TRequest, TResponse>(TRequest request, string serviceName, string methodName)
            where TRequest : RequestBase
            where TResponse : ResponseBase
        {
            if (request == null)
            {
                return default(TResponse);
            }

            if (request.Head == null)
            {
                request.Head = new RequestHead();
            }

            request.Head.Token = Token;

            var url = $"{BaseUrl}/{serviceName}/{methodName}";

            try
            {
                using (var response = await _HttpManager.Create().PostAsync(url, null, request, _Formatter, null))
                {
                    if (response != null && response.StatusCode == 200)
                    {
                        return await response.GetBodyAsync<TResponse>(_Formatter);
                    }
                    else
                    {
                        _Logger.LogEvent(nameof(DefaultRestfulClient), Severity.Error, await BuildErrorDesc(url, request, response));
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.LogEvent(nameof(DefaultRestfulClient), Severity.Error, e, await BuildErrorDesc(url, request, null));
            }

            return default(TResponse);
        }

        private async Task<string> BuildErrorDesc(string url, object requestBody, IHttpResponse response)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"URL: {url}");
            if (response != null)
            {
                stringBuilder.AppendLine($"StatusCode: {response.StatusCode}");
            }
            stringBuilder.AppendLine("================ Request ================");
            stringBuilder.AppendLine(await _Formatter.WriteObjectAsync(requestBody));
            stringBuilder.AppendLine("================ Response ================");
            if (response == null)
            {
                stringBuilder.AppendLine("response is null.");
            }
            else
            {
                try
                {
                    stringBuilder.AppendLine(await response.GetStringAsync());
                }
                catch (Exception)
                {
                    stringBuilder.AppendLine("error occured when reading response.");
                }
            }
            return stringBuilder.ToString();
        }
    }
}