using Guru.Formatter.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Guru.Network.Abstractions
{
    public interface IHttpResponse : IDisposable
    {
        int StatusCode { get; }

        IReadOnlyDictionary<string, string[]> Headers { get; }

        long ContentLength { get; }

        string Location { get; }

        RequestUri RequestUri { get; }

        Task<TBody> GetBodyAsync<TBody>(ILightningFormatter formatter);

        Task GetBodyAsync(Func<byte[], int, int, Task> handler, int bufferSize = 4 * 1024);

        Task<Stream> GetStreamAsync();

        Task<string> GetStringAsync();
    }
}