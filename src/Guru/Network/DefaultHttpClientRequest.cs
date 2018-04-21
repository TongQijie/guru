using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
 
using Guru.Utils;
using Guru.ExtensionMethod;
using Guru.Network.Abstractions;
using Guru.Formatter.Abstractions;

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

        public async Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString, object body, TFormatter formatter, Dictionary<string, string> contentHeaders = null) where TFormatter : ILightningFormatter
        {
            if (queryString != null)
            {
                uri = AddQueryString(uri, queryString);
            }

            if (contentHeaders == null || !contentHeaders.ContainsKey("Content-Type"))
            {
                if (contentHeaders == null)
                {
                    contentHeaders = new Dictionary<string, string>();
                }
                if (formatter.Tag.EqualsIgnoreCase("JSON"))
                {
                    contentHeaders.Add("Content-Type", "application/json");
                }
                else if (formatter.Tag.EqualsIgnoreCase("XML"))
                {
                    contentHeaders.Add("Content-Type", "application/xml");
                }
            }

            return await InternalPostAsync(uri, body, formatter, contentHeaders);
        }

        public async Task<IHttpClientResponse> PostAsync(string uri, IDictionary<string, string> queryString, Dictionary<string, string> formData, Dictionary<string, string> contentHeaders = null)
        {
            var body = string.Empty;
            if (formData != null)
            {
                 body = string.Join("&", formData.Select(x => $"{WebUtils.UrlEncode(x.Key)}={WebUtils.UrlEncode(x.Value)}"));
            }

            if (queryString != null)
            {
                uri = AddQueryString(uri, queryString);
            }

            if (contentHeaders == null || !contentHeaders.ContainsKey("Content-Type"))
            {
                if (contentHeaders == null)
                {
                    contentHeaders = new Dictionary<string, string>();
                }
                contentHeaders.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            }

            return await InternalPostAsync(uri, Encoding.UTF8.GetBytes(body), contentHeaders);
        }

        public async Task<IHttpClientResponse> PostAsync(string uri, IDictionary<string, string> queryString, byte[] byteArrayContent, Dictionary<string, string> contentHeaders = null)
        {
            if (queryString != null)
            {
                uri = AddQueryString(uri, queryString);
            }

            return await InternalPostAsync(uri, byteArrayContent, contentHeaders);
        }

        private string AddQueryString(string uri, IDictionary<string, string> queryString)
        {
            //uri = uri.TrimEnd('/', '?');

            if (uri.ContainsIgnoreCase("?"))
            {
                uri = uri + "&";
            }
            else
            {
                uri = uri + "?";
            }

            return uri + string.Join("&", queryString.Where(x => x.Key.HasValue())
                .Select(x => x.Value.HasValue() ? 
                    $"{WebUtils.UrlEncode(x.Key)}={WebUtils.UrlEncode(x.Value)}" : 
                    $"{WebUtils.UrlEncode(x.Key)}"));
        }

        private async Task<IHttpClientResponse> InternalPostAsync<TFormatter>(string uri, object body, TFormatter formatter, Dictionary<string, string> contentHeaders) where TFormatter : ILightningFormatter
        {
            byte[] byteArrayContent;
            using (var memoryStream = new MemoryStream())
            {
                await formatter.WriteObjectAsync(body, memoryStream);
                byteArrayContent = memoryStream.ToArray();
            }

            return await InternalPostAsync(uri, byteArrayContent, contentHeaders);
        }

        private async Task<IHttpClientResponse> InternalPostAsync(string uri, byte[] byteArrayContent, Dictionary<string, string> contentHeaders)
        {
            using (var content = new ByteArrayContent(byteArrayContent))
            {
                content.Headers.ContentLength = byteArrayContent.Length;

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