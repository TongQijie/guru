using Guru.Formatter.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guru.Network.Abstractions
{
    public interface IHttpSession
    {
        Task<IHttpResponse> GetAsync(string url, IDictionary<string, string> queryString, IDictionary<string, string> headers = null);

        Task<IHttpResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString,
            object body, TFormatter formatter,
            IDictionary<string, string> headers = null) where TFormatter : ILightningFormatter;

        Task<IHttpResponse> PostAsync(string uri, IDictionary<string, string> queryString,
            IDictionary<string, string> formData,
            IDictionary<string, string> headers = null);

        Task<IHttpResponse> PostAsync(string uri, IDictionary<string, string> queryString,
            byte[] byteArrayContent,
            IDictionary<string, string> headers = null);
    }
}
