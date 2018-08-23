using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Http.Abstractions;

namespace Guru.Http.Implementation
{
    [Injectable(typeof(IHttpManager), Lifetime.Singleton)]
    internal class DefaultHttpManager : IHttpManager
    {
        private ConcurrentDictionary<string, IHttpRequest> _Requests = new ConcurrentDictionary<string, IHttpRequest>();

        public IHttpRequest Create()
        {
            IHttpRequest request;
            if (!_Requests.TryGetValue("Default", out request))
            {
                request = DependencyContainer.Resolve<IHttpRequest>();
                _Requests.AddOrUpdate("Default", request, (i, b) => request);
            }
            return request;
        }

        public IHttpRequest Create(IWebProxy webProxy, bool ignoredCertificateValidation, TimeSpan? timeout)
        {
            if (webProxy == null && !ignoredCertificateValidation && timeout == null)
            {
                return Create();
            }

            var stringBuilder = new StringBuilder();
            if (webProxy != null)
            {
                stringBuilder.Append(webProxy.GetHashCode());
            }
            if (ignoredCertificateValidation)
            {
                stringBuilder.Append(ignoredCertificateValidation);
            }
            if (timeout != null)
            {
                stringBuilder.Append(timeout.Value.TotalMilliseconds);
            }

            IHttpRequest request;
            if (!_Requests.TryGetValue(stringBuilder.ToString(), out request))
            {
                request = DependencyContainer.Resolve<IHttpRequest>();
                request.Configure(webProxy, ignoredCertificateValidation, timeout);
                _Requests.AddOrUpdate(stringBuilder.ToString(), request, (i, b) => request);
            }

            return request;
        }
    }
}
