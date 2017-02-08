using System.Collections.Concurrent;

using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Network
{
    [DI(typeof(IHttpClientBroker), Lifetime = Lifetime.Singleton)]
    public class DefaultHttpClientBroker : IHttpClientBroker
    {
        private ConcurrentDictionary<string, IHttpClientRequest> _Requests = new ConcurrentDictionary<string, IHttpClientRequest>();

        public IHttpClientRequest Get()
        {
            IHttpClientRequest request;
            if (!_Requests.TryGetValue(DefaultHttpClientSettings.DefaultSettingId, out request))
            {
                request = new DefaultHttpClientRequest(new DefaultHttpClientSettings(null, null, null, null));
                _Requests.AddOrUpdate(DefaultHttpClientSettings.DefaultSettingId, request, (i, b) => request);
            }

            return request;
        }

        public IHttpClientRequest Get(string id)
        {
            IHttpClientRequest request;
            if (!_Requests.TryGetValue(id, out request))
            {
                return null;
            }

            return request;
        }

        public IHttpClientRequest Get(IHttpClientSettings settings)
        {
            IHttpClientRequest request;
            if (!_Requests.TryGetValue(settings.Id, out request))
            {
                request = new DefaultHttpClientRequest(settings);
                _Requests.AddOrUpdate(settings.Id, request, (i, b) => request);
            }

            return request;
        }
    }
}