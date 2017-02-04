using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.WebUtilities;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;

namespace Guru.Network
{
    public class HttpBroker : IDisposable
    {
        private readonly HttpClient _Client;

        private readonly string _Uri;

        private HttpContent _ResponseContent;

        private HttpBroker()
        {
            _Client = new HttpClient();
        }

        public HttpBroker(string uri) : this()
        {
            _Uri = uri;
        }

        public HttpBroker(string uri, IDictionary<string, string> queryString) : this(uri)
        {
            _Uri = QueryHelpers.AddQueryString(uri, queryString);
        }

        public async Task<int> GetAsync()
        {
            var response = await _Client.GetAsync(_Uri);

            _ResponseContent = response.Content;

            return (int)response.StatusCode;
        }

        public async Task<int> PostAsync<TFormatter>(object body) where TFormatter : IFormatter
        {
            var bytes = ContainerEntry.Resolve<TFormatter>().WriteBytes(body);

            using (var content = new ByteArrayContent(bytes))
            {
                content.Headers.ContentLength = bytes.Length;

                var response = await _Client.PostAsync(_Uri, content);

                 _ResponseContent = response.Content;

                return (int)response.StatusCode;
            }
        }

        public async Task<TBody> GetBodyAsync<TBody, TFormatter>() where TFormatter : IFormatter
        {
            using (var stream = await _ResponseContent.ReadAsStreamAsync())
            {
                return await ContainerEntry.Resolve<TFormatter>().ReadObjectAsync<TBody>(stream);
            }
        }

        public void Dispose()
        {
            if (_ResponseContent != null)
            {
                _ResponseContent.Dispose();
                _ResponseContent = null;
            }

            _Client.Dispose();
        }
    }
}