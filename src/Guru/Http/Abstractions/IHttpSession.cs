using Guru.Formatter.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Guru.Http.Abstractions
{
    public interface IHttpSession
    {
        bool LocationEnabled { get; set; }

        IHttpSession Configure(IWebProxy webProxy, bool ignoredCertificateValidation, TimeSpan? timeout);

        Task<IHttpResponse> GetAsync(string url, RequestParams requestParams);

        Task<IHttpResponse> PostAsync<TFormatter>(string url, object body, TFormatter formatter, RequestParams requestParams) where TFormatter : ILightningFormatter;

        Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> formData, RequestParams requestParams);

        Task<IHttpResponse> PostAsync(string url, byte[] byteArrayContent, RequestParams requestParams);
    }
}
