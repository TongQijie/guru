using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Formatter.Abstractions;

namespace Guru.Network
{
    public interface IHttpClientResponse : IDisposable
    {
        int StatusCode { get; }

        IReadOnlyDictionary<string, string[]> Headers { get; }

        Task<TBody> GetBodyAsync<TBody, TFormatter>() where TFormatter : IFormatter;

        Task GetBodyAsync(Func<byte[], int, int, Task> handler, int bufferSize = 4 * 1024);
    }
}