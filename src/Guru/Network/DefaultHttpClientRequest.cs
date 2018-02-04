 using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
 
using Guru.Utils;
using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Network.Abstractions;
using Guru.Formatter.Abstractions;
using System.IO;

namespace Guru.Network
{
    public class DefaultHttpClientRequest : IHttpClientRequest
    {
        private readonly IHttpClientSettings _Settings;

        private readonly HttpClient _Client;

        public DefaultHttpClientRequest(IHttpClientSettings settings)
        {
            _Settings = settings;

            var handler = new HttpClientHandler();
            
            if (_Settings.Proxy != null)
            {
                handler.Proxy = _Settings.Proxy;
                handler.Credentials = _Settings.Proxy.Credentials;
            }
            
            _Client = new HttpClient(handler);

            if (_Settings.Timeout != null)
            {
                _Client.Timeout = (TimeSpan)_Settings.Timeout;
            }

            if (settings.Headers != null)
            {
                foreach (var header in settings.Headers)
                {
                    _Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public async Task<IHttpClientResponse> GetAsync(string uri)
        {
            return new DefaultHttpClientResponse(await _Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead));
        }

        public async Task<IHttpClientResponse> GetAsync(string uri, IDictionary<string, string> queryString)
        {
            if (queryString != null)
            {
                uri = AddQueryString(uri, queryString);
            }

            return new DefaultHttpClientResponse(await _Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead));
        }

        public async Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, object body, Dictionary<string, string> contentHeaders = null) where TFormatter : ILightningFormatter
        {
            return await InternalPostAsync<TFormatter>(uri, body, contentHeaders); 
        }

        public async Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString, object body, Dictionary<string, string> contentHeaders = null) where TFormatter : ILightningFormatter
        {
            if (queryString != null)
            {
                uri = AddQueryString(uri, queryString);
            }

            return await InternalPostAsync<TFormatter>(uri, body, contentHeaders);
        }

        public async Task<IHttpClientResponse> PostAsync(string uri, IDictionary<string, string> queryString, Dictionary<string, string> formData, Dictionary<string, string> contentHeaders = null)
        {
            var body = string.Empty;
            if (formData != null)
            {
                 body = string.Join("&", formData.Select(x => $"{WebUtils.UrlEncode(x.Key)}={WebUtils.UrlEncode(x.Value)}"));
            }

            return await PostAsync<ILightningFormatter>(uri, queryString, body, contentHeaders);
        }

        private string AddQueryString(string uri, IDictionary<string, string> queryString)
        {
            uri = uri.TrimEnd('/', '?');

            if (uri.ContainsIgnoreCase("?"))
            {
                uri = uri + "&";
            }
            else
            {
                uri = uri + "?";
            }

            return uri + string.Join("&", queryString.Select(x => $"{WebUtils.UrlEncode(x.Key)}={WebUtils.UrlEncode(x.Value)}"));
        }

        private async Task<IHttpClientResponse> InternalPostAsync<TFormatter>(string uri, object body, Dictionary<string, string> contentHeaders) where TFormatter : ILightningFormatter
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                await DependencyContainer.Resolve<TFormatter>().WriteObjectAsync(body, memoryStream);
                bytes = memoryStream.ToArray();
            }

            using (var content = new ByteArrayContent(bytes))
            {
                content.Headers.ContentLength = bytes.Length;

                if (contentHeaders != null)
                {
                    foreach (var contentHeader in contentHeaders)
                    {
                        if (contentHeader.Key.EqualsIgnoreCase("Content-Type") && MediaTypeHeaderValue.TryParse(contentHeader.Value, out var parsedValue))
                        {
                            content.Headers.ContentType = parsedValue;
                        }
                    }
                }

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
                requestMessage.Content = content;

                return new DefaultHttpClientResponse(await _Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));
            }
        }
    }
}