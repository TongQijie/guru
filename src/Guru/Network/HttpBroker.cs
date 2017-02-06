using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.WebUtilities;

using Guru.ExtensionMethod;
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
        
        public HttpBroker SetHeader(string name, string value)
        {
            _Client.DefaultRequestHeaders.Add(name, new string[] { value });
            return this;
        }

        public HttpBroker SetTimeout(TimeSpan timeout)
        {
            _Client.Timeout = timeout;
            return this;
        }

        public async Task<int> GetAsync()
        {
            var response = await _Client.GetAsync(_Uri, HttpCompletionOption.ResponseHeadersRead);

            _ResponseContent = response.Content;

            return (int)response.StatusCode;
        }

        public async Task<int> PostAsync<TFormatter>(object body) where TFormatter : IFormatter
        {
            var bytes = ContainerEntry.Resolve<TFormatter>().WriteBytes(body);

            using (var content = new ByteArrayContent(bytes))
            {
                content.Headers.ContentLength = bytes.Length;

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, _Uri);
                requestMessage.Content = content;

                var response = await _Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

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

        public async Task GetBodyAsync(Func<byte[], int, int, Task> handler, int bufferSize = 4 * 1024)
        {
            var buffer = new byte[bufferSize];
            int count = 0;

            using (var stream = await _ResponseContent.ReadAsStreamAsync())
            {
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await handler(buffer, 0, count);
                }
            }
        }

        private Dictionary<string, string> _ResponseHeaders;

        public IReadOnlyDictionary<string, string> ResponseHeaders
        {
            get
            {
                if (_ResponseContent != null && _ResponseHeaders == null)
                {
                    _ResponseHeaders = new Dictionary<string, string>();

                    foreach (var header in _ResponseContent.Headers)
                    {
                        foreach (var value in header.Value)
                        {
                            _ResponseHeaders.Add(header.Key.ToLower(), value);
                        }
                    }
                }

                return _ResponseHeaders;
            }
        }

        public T GetResponseHeader<T>(string name)
        {
            if (ResponseHeaders != null && ResponseHeaders.ContainsKey(name.ToLower()))
            {
                return ResponseHeaders[name.ToLower()].ConvertTo<T>();
            }

            return default(T);
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