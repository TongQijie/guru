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
using Guru.Logging;
using Guru.Logging.Abstractions;
using Guru.Network.Abstractions;
using Guru.Utils;

namespace Guru.Network.Implementation
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
                    };

                    if (_WebProxy != null)
                    {
                        handler.Proxy = _WebProxy;
                        handler.Credentials = _WebProxy.Credentials;   
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

        private readonly ILogger _Logger;

        public DefaultHttpRequest(IFileLogger logger)
        {
            _Logger = logger;
        }

        public void Configure(IWebProxy webProxy, TimeSpan? timeout)
        {
            _WebProxy = webProxy;
            _Timeout = timeout;
        }

        public async Task<IHttpResponse> GetAsync(string url, IDictionary<string, string> queryString, IDictionary<string, string> headers = null)
        {
            return await InternalGetAsync(AppendQueryString(url, queryString), headers);
        }

        public async Task<IHttpResponse> PostAsync<TFormatter>(string url, IDictionary<string, string> queryString, object body, TFormatter formatter, IDictionary<string, string> headers = null) where TFormatter : ILightningFormatter
        {
            if (headers == null || !headers.ContainsKey("Content-Type"))
            {
                if (headers == null)
                {
                    headers = new Dictionary<string, string>();
                }
                if (formatter.Tag.EqualsIgnoreCase("JSON"))
                {
                    headers.Add("Content-Type", "application/json");
                }
                else if (formatter.Tag.EqualsIgnoreCase("XML"))
                {
                    headers.Add("Content-Type", "application/xml");
                }
            }

            byte[] byteArrayContent;
            using (var memoryStream = new MemoryStream())
            {
                await formatter.WriteObjectAsync(body, memoryStream);
                byteArrayContent = memoryStream.ToArray();
            }

            return await InternalPostAsync(AppendQueryString(url, queryString), byteArrayContent, headers);
        }

        public async Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> queryString, IDictionary<string, string> formData, IDictionary<string, string> headers = null)
        {
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

            if (headers == null || !headers.ContainsKey("Content-Type"))
            {
                if (headers == null)
                {
                    headers = new Dictionary<string, string>();
                }

                headers.Add("Content-Type", "application/x-www-form-urlencoded");
            }

            return await InternalPostAsync(AppendQueryString(url, queryString), byteArrayContent, headers);
        }

        public async Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> queryString, byte[] byteArrayContent, IDictionary<string, string> headers = null)
        {
            return await InternalPostAsync(AppendQueryString(url, queryString), byteArrayContent, headers);
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

        private async Task<IHttpResponse> InternalGetAsync(string url, IDictionary<string, string> headers)
        {
            var requestMessage = AppendHeaders(new HttpRequestMessage(HttpMethod.Get, url), headers);
            try
            {
                return new DefaultHttpResponse(await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));
            }
            catch (Exception e)
            {
                _Logger.LogEvent(nameof(DefaultHttpRequest), Severity.Error, $"failed to request: {url}", e);
            }

            return null;
        }

        private async Task<IHttpResponse> InternalPostAsync(string url, byte[] byteArrayContent, IDictionary<string, string> headers)
        {
            using (var content = new ByteArrayContent(byteArrayContent))
            {
                content.Headers.ContentLength = byteArrayContent.Length;
                var contentTypeKey = headers.Keys.FirstOrDefault(x => x.EqualsIgnoreCase("Content-Type"));
                if (contentTypeKey != null)
                {
                    if (MediaTypeHeaderValue.TryParse(headers[contentTypeKey], out var parsedValue))
                    {
                        content.Headers.ContentType = parsedValue;
                    }
                    headers.Remove(contentTypeKey);
                }
                var requestMessage = AppendHeaders(new HttpRequestMessage(HttpMethod.Post, url), headers);
                requestMessage.Content = content;
                try
                {
                    return new DefaultHttpResponse(await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));
                }
                catch (Exception e)
                {
                    _Logger.LogEvent(nameof(DefaultHttpRequest), Severity.Error, $"failed to request: {url}", e);
                }

                return null;
            }
        }
    }
}