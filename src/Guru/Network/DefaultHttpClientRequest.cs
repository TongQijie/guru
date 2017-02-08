using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.WebUtilities;

using Guru.DependencyInjection;
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

        public async Task<IHttpClientResponse> GetAsync(string uri, IDictionary<string, string> queryString)
        {
            if (queryString != null)
            {
                uri = QueryHelpers.AddQueryString(uri, queryString);
            }

            return new DefaultHttpClientResponse(await _Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead));
        }

        public async Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString, object body) where TFormatter : IFormatter
        {
            if (queryString != null)
            {
                uri = QueryHelpers.AddQueryString(uri, queryString);
            }

            var bytes = ContainerEntry.Resolve<TFormatter>().WriteBytes(body);

            using (var content = new ByteArrayContent(bytes))
            {
                content.Headers.ContentLength = bytes.Length;

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
                requestMessage.Content = content;

                return new DefaultHttpClientResponse(await _Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));
            }
        }
    }
}