using Guru.Formatter.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Guru.Network.Abstractions
{
    public interface IHttpRequest
    {
        void Configure(IWebProxy webProxy, bool ignoredCertificateValidation, TimeSpan? timeout);

        Task<IHttpResponse> GetAsync(string url, IDictionary<string, string> queryString, IDictionary<string, string> headers = null);

        Task<IHttpResponse> PostAsync<TFormatter>(string url, IDictionary<string, string> queryString, 
            object body, TFormatter formatter, 
            IDictionary<string, string> headers = null) where TFormatter : ILightningFormatter;

        Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> queryString, 
            IDictionary<string, string> formData, 
            IDictionary<string, string> headers = null);

        Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> queryString, 
            byte[] byteArrayContent, 
            IDictionary<string, string> headers = null);
    }
}