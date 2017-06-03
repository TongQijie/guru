using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Formatter.Abstractions;

namespace Guru.Network.Abstractions
{
    public interface IHttpClientRequest
    {
         Task<IHttpClientResponse> GetAsync(string uri);

         Task<IHttpClientResponse> GetAsync(string uri, IDictionary<string, string> queryString);

         Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, object body, Dictionary<string, string> contentHeaders = null) where TFormatter : IFormatter;

         Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString, object body, Dictionary<string, string> contentHeaders = null) where TFormatter : IFormatter;

         Task<IHttpClientResponse> PostAsync(string uri, IDictionary<string, string> queryString, Dictionary<string, string> formData, Dictionary<string, string> contentHeaders = null);
    }
}