using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Formatter.Abstractions;

namespace Guru.Network
{
    public interface IHttpClientRequest
    {
         Task<IHttpClientResponse> GetAsync(string uri, IDictionary<string, string> queryString);

         Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString, object body) where TFormatter : IFormatter;
    }
}