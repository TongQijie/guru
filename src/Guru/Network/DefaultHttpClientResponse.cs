using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.DependencyInjection;
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

        public IReadOnlyDictionary<string, string[]> Headers
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public async Task<TBody> GetBodyAsync<TBody, TFormatter>() where TFormatter : IFormatter
        {
            using (var stream = await _Response.Content.ReadAsStreamAsync())
            {
                return await ContainerEntry.Resolve<TFormatter>().ReadObjectAsync<TBody>(stream);
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

        public void Dispose()
        {
            _Response.Dispose();
        }
    }
}