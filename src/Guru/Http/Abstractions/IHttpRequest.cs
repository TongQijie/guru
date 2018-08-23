using Guru.Formatter.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Guru.Http.Abstractions
{
    public interface IHttpRequest
    {
        IHttpRequest Configure(IWebProxy webProxy, bool ignoredCertificateValidation, TimeSpan? timeout);

        Task<IHttpResponse> GetAsync(string url, RequestParams requestParams);

        Task<IHttpResponse> PostAsync<TFormatter>(string url, object body, TFormatter formatter, RequestParams requestParams) where TFormatter : ILightningFormatter;

        Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> formData, RequestParams requestParams);

        Task<IHttpResponse> PostAsync(string url, byte[] byteArrayContent, RequestParams requestParams);

        Task<IHttpResponse> PutAsync<TFormatter>(string url, object body, TFormatter formatter, RequestParams requestParams) where TFormatter : ILightningFormatter;

        Task<IHttpResponse> PutAsync(string url, IDictionary<string, string> formData, RequestParams requestParams);

        Task<IHttpResponse> PutAsync(string url, byte[] byteArrayContent, RequestParams requestParams);
    }
}