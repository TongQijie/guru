using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Http.Abstractions;
using Guru.Logging;
using Guru.Logging.Abstractions;
using Guru.Utils;

namespace Guru.Http.Implementation
{
    [Injectable(typeof(IHttpRequest), Lifetime.Transient)]
    internal class DefaultHttpRequest : IHttpRequest
    {
        private HttpClient _Client = null;

        private HttpClient Client
        {
            get
            {
                if (_Client == null)
                {
                    var handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                        AllowAutoRedirect = false,
                    };

                    if (_WebProxy != null)
                    {
                        handler.Proxy = _WebProxy;
                        handler.Credentials = _WebProxy.Credentials;
                    }

                    if (_IgnoredCertificateValidation)
                    {
                        handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    }

                    _Client = new HttpClient(handler);

                    if (_Timeout != null)
                    {
                        _Client.Timeout = (TimeSpan)_Timeout;
                    }
                }
                return _Client;
            }
        }

        private IWebProxy _WebProxy = null;

        private TimeSpan? _Timeout = null;

        private bool _IgnoredCertificateValidation = false;

        private readonly ILogger _Logger;

        public DefaultHttpRequest(IFileLogger logger)
        {
            _Logger = logger;
        }

        public IHttpRequest Configure(IWebProxy webProxy, bool ignoredCertificateValidation, TimeSpan? timeout)
        {
            _WebProxy = webProxy;
            _IgnoredCertificateValidation = ignoredCertificateValidation;
            _Timeout = timeout;
            return this;
        }

        public async Task<IHttpResponse> GetAsync(string url, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            return await HttpInvokeAsync(HttpMethod.Get, AppendQueryString(url, requestParams.QueryString), requestParams.Headers, null);
        }

        public async Task<IHttpResponse> PostAsync<TFormatter>(string url, object body, TFormatter formatter, RequestParams requestParams) where TFormatter : ILightningFormatter
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            if (requestParams.Headers == null || !requestParams.Headers.ContainsKey("Content-Type"))
            {
                if (requestParams.Headers == null)
                {
                    requestParams.Headers = new Dictionary<string, string>();
                }
                if (formatter != null && formatter.Tag.EqualsIgnoreCase("JSON"))
                {
                    requestParams.Headers.Add("Content-Type", "application/json");
                }
                else if (formatter != null && formatter.Tag.EqualsIgnoreCase("XML"))
                {
                    requestParams.Headers.Add("Content-Type", "application/xml");
                }
            }

            byte[] byteArrayContent;
            if (body == null)
            {
                byteArrayContent = new byte[0];
            }
            else if (body is string)
            {
                byteArrayContent = Encoding.UTF8.GetBytes(body.ToString());
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formatter.WriteObjectAsync(body, memoryStream);
                    byteArrayContent = memoryStream.ToArray();
                }
            }

            return await HttpInvokeAsync(HttpMethod.Post, AppendQueryString(url, requestParams.QueryString), requestParams.Headers, byteArrayContent);
        }

        public async Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> formData, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            byte[] byteArrayContent = new byte[0];
            if (formData != null && formData.Count > 0)
            {
                var stringBuilder = new StringBuilder();
                foreach (var kv in formData)
                {
                    if (!kv.Key.HasValue())
                    {
                        continue;
                    }
                    stringBuilder.Append($"{WebUtils.UrlEncode(kv.Key)}={WebUtils.UrlEncode(kv.Value)}&");
                }
                byteArrayContent = Encoding.UTF8.GetBytes(stringBuilder.ToString().TrimEnd('&'));
            }

            if (requestParams.Headers == null || !requestParams.Headers.ContainsKey("Content-Type"))
            {
                if (requestParams.Headers == null)
                {
                    requestParams.Headers = new Dictionary<string, string>();
                }

                requestParams.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            }

            return await HttpInvokeAsync(HttpMethod.Post, AppendQueryString(url, requestParams.QueryString), requestParams.Headers, byteArrayContent);
        }

        public async Task<IHttpResponse> PostAsync(string url, byte[] byteArrayContent, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            return await HttpInvokeAsync(HttpMethod.Post, AppendQueryString(url, requestParams.QueryString), requestParams.Headers, byteArrayContent);
        }

        public async Task<IHttpResponse> PutAsync<TFormatter>(string url, object body, TFormatter formatter, RequestParams requestParams) where TFormatter : ILightningFormatter
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            if (requestParams.Headers == null || !requestParams.Headers.ContainsKey("Content-Type"))
            {
                if (requestParams.Headers == null)
                {
                    requestParams.Headers = new Dictionary<string, string>();
                }
                if (formatter != null && formatter.Tag.EqualsIgnoreCase("JSON"))
                {
                    requestParams.Headers.Add("Content-Type", "application/json");
                }
                else if (formatter != null && formatter.Tag.EqualsIgnoreCase("XML"))
                {
                    requestParams.Headers.Add("Content-Type", "application/xml");
                }
            }

            byte[] byteArrayContent;
            if (body == null)
            {
                byteArrayContent = new byte[0];
            }
            else if (body is string)
            {
                byteArrayContent = Encoding.UTF8.GetBytes(body.ToString());
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formatter.WriteObjectAsync(body, memoryStream);
                    byteArrayContent = memoryStream.ToArray();
                }
            }

            return await HttpInvokeAsync(HttpMethod.Put, AppendQueryString(url, requestParams.QueryString), requestParams.Headers, byteArrayContent);
        }

        public async Task<IHttpResponse> PutAsync(string url, IDictionary<string, string> formData, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            byte[] byteArrayContent = new byte[0];
            if (formData != null && formData.Count > 0)
            {
                var stringBuilder = new StringBuilder();
                foreach (var kv in formData)
                {
                    if (!kv.Key.HasValue())
                    {
                        continue;
                    }
                    stringBuilder.Append($"{WebUtils.UrlEncode(kv.Key)}={WebUtils.UrlEncode(kv.Value)}&");
                }
                byteArrayContent = Encoding.UTF8.GetBytes(stringBuilder.ToString().TrimEnd('&'));
            }

            if (requestParams.Headers == null || !requestParams.Headers.ContainsKey("Content-Type"))
            {
                if (requestParams.Headers == null)
                {
                    requestParams.Headers = new Dictionary<string, string>();
                }

                requestParams.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            }

            return await HttpInvokeAsync(HttpMethod.Put, AppendQueryString(url, requestParams.QueryString), requestParams.Headers, byteArrayContent);
        }

        public async Task<IHttpResponse> PutAsync(string url, byte[] byteArrayContent, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            return await HttpInvokeAsync(HttpMethod.Put, AppendQueryString(url, requestParams.QueryString), requestParams.Headers, byteArrayContent);
        }

        private string AppendQueryString(string url, IDictionary<string, string> queryString)
        {
            if (queryString == null || queryString.Count == 0)
            {
                return url;
            }

            var stringBuilder = new StringBuilder(url);
            if (url.ContainsIgnoreCase("?"))
            {
                stringBuilder.Append("&");
            }
            else
            {
                stringBuilder.Append("?");
            }

            foreach (var kv in queryString)
            {
                if (!kv.Key.HasValue())
                {
                    continue;
                }

                if (kv.Value.HasValue())
                {
                    stringBuilder.Append($"{WebUtils.UrlEncode(kv.Key)}={WebUtils.UrlEncode(kv.Value)}&");
                }
                else
                {
                    stringBuilder.Append($"{WebUtils.UrlEncode(kv.Key)}&");
                }
            }

            return stringBuilder.ToString().TrimEnd('&');
        }

        private HttpRequestMessage AppendHeaders(HttpRequestMessage httpRequestMessage, IDictionary<string, string> headers)
        {
            if (headers == null || headers.Count == 0)
            {
                return httpRequestMessage;
            }

            foreach (var kv in headers)
            {
                if (!kv.Key.HasValue())
                {
                    continue;
                }

                httpRequestMessage.Headers.Add(kv.Key, kv.Value);
            }

            return httpRequestMessage;
        }

        private async Task<IHttpResponse> HttpInvokeAsync(HttpMethod method, string url, IDictionary<string, string> headers, byte[] body)
        {
            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                using (var content = CreateHttpContent(headers, body))
                {
                    var requestMessage = AppendHeaders(new HttpRequestMessage(method, url), headers);
                    requestMessage.Content = content;
                    try
                    {
                        return new DefaultHttpResponse(await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));
                    }
                    catch (Exception e)
                    {
                        _Logger.LogEvent(nameof(DefaultHttpRequest), Severity.Error, $"failed to request: {url}", e);
                    }
                }
            }
            else
            {
                var requestMessage = AppendHeaders(new HttpRequestMessage(method, url), headers);
                try
                {
                    return new DefaultHttpResponse(await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));
                }
                catch (Exception e)
                {
                    _Logger.LogEvent(nameof(DefaultHttpRequest), Severity.Error, $"failed to request: {url}", e);
                }
            }

            return null;
        }

        private HttpContent CreateHttpContent(IDictionary<string, string> headers, byte[] body)
        {
            var httpContent = new ByteArrayContent(body ?? new byte[0]);

            if (headers != null)
            {
                foreach (var key in headers.Keys.ToArray())
                {
                    if (key.EqualsIgnoreCase("Content-Type"))
                    {
                        if (MediaTypeHeaderValue.TryParse(headers[key], out var contentType))
                        {
                            httpContent.Headers.ContentType = contentType;
                        }
                        headers.Remove(key);
                    }
                }
            }

            if (body != null)
            {
                httpContent.Headers.ContentLength = body.Length;
            }

            return httpContent;
        }
    }
}