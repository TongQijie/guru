using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.DependencyInjection;
using Guru.Network.Abstractions;
using Guru.Formatter.Abstractions;

namespace Guru.Network
{
    public class DefaultHttpClientResponse : IHttpClientResponse
    {
        private readonly HttpResponseMessage _Response;

        public DefaultHttpClientResponse(HttpResponseMessage response)
        {
            _Response = response;
        }

        public int StatusCode => (int)_Response?.StatusCode;

        private Dictionary<string, string[]> _Headers;

        public IReadOnlyDictionary<string, string[]> Headers
        {
            get
            {
                if (_Headers == null)
                {
                    _Headers = new Dictionary<string, string[]>();

                    if (_Response.Headers != null)
                    {
                        foreach (var header in _Response.Headers)
                        {
                            _Headers.Add(header.Key, header.Value.ToArray());
                        }
                    }

                    if (_Response.Content != null && _Response.Content.Headers != null)
                    {
                        foreach (var header in _Response.Content.Headers)
                        {
                            _Headers.Add(header.Key, header.Value.ToArray());
                        }
                    }
                }

                return _Headers;
            }
        }
        
        public async Task<TBody> GetBodyAsync<TBody, TFormatter>() where TFormatter : IFormatter
        {
            using (var stream = await _Response.Content.ReadAsStreamAsync())
            {
                return await ContainerEntry.Resolve<TFormatter>().ReadObjectAsync<TBody>(stream);
            }
        }

        public async Task<TBody> GetBodyAsync<TBody>(IFormatter formatter)
        {
            using (var stream = await _Response.Content.ReadAsStreamAsync())
            {
                return await formatter.ReadObjectAsync<TBody>(stream);
            }
        }

        public async Task GetBodyAsync(Func<byte[], int, int, Task> handler, int bufferSize = 4096)
        {
            var buffer = new byte[bufferSize];
            int count = 0;

            using (var stream = await _Response.Content.ReadAsStreamAsync())
            {
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await handler(buffer, 0, count);
                }
            }
        }

        public async Task<Stream> GetStream()
        {
            return await _Response.Content.ReadAsStreamAsync();
        }

        public void Dispose()
        {
            _Response.Dispose();
        }
    }
}