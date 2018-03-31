using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Formatter.Abstractions;

namespace Guru.Network.Abstractions
{
    public interface IHttpClientResponse : IDisposable
    {
        int StatusCode { get; }

        IReadOnlyDictionary<string, string[]> Headers { get; }

        long ContentLength { get; }
        
        Task<TBody> GetBodyAsync<TBody>(ILightningFormatter formatter);

        Task GetBodyAsync(Func<byte[], int, int, Task> handler, int bufferSize = 4 * 1024);

        Task<Stream> GetStreamAsync();

        Task<string> GetStringAsync();
    }
}