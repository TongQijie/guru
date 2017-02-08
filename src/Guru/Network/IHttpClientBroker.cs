using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Formatter.Abstractions;

namespace Guru.Network
{
    public interface IHttpClientBroker
    {
         IHttpClientBroker Get();

         IHttpClientBroker Get(string id);

         IHttpClientBroker Get(IHttpClientSettings settings);

         Task<IHttpClientResponse> GetAsync(string uri, IDictionary<string, string> queryString);

         Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString, object body) where TFormatter : IFormatter;
    }
}