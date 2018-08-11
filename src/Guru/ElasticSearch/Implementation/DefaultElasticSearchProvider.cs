using System.Threading.Tasks;
using System.Linq;
using Guru.Network;
using Guru.Formatter.Abstractions;
using Guru.Network.Abstractions;
using Guru.ElasticSearch.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;

namespace Guru.ElasticSearch.Implementation
{
    [Injectable(typeof(IElasticSearchProvider), Lifetime.Transient)]
    internal class DefaultElasticSearchProvider : IElasticSearchProvider
    {
        private readonly IHttpRequest _HttpRequest;

        private readonly ILightningFormatter _LightningFormatter;

        public DefaultElasticSearchProvider(IHttpManager httpManager, IJsonLightningFormatter jsonLightningFormatter)
        {
            _HttpRequest = httpManager.Create();
            _LightningFormatter = jsonLightningFormatter;
        }

        public string HostUrl { get; private set; }

        public void Configure(string hostUrl)
        {
            HostUrl = hostUrl;
        }

        public async Task<bool> CreateIndex(string index)
        {
            using (var httpResponse = await _HttpRequest.PutAsync(CreateUrlByCreateIndex(index), null, new byte[0], null))
            {
                return httpResponse.IsHttpCreated();
            }
        }

        public async Task<bool> PutDocument(string index, string type, string id, object document)
        {
            using (var httpResponse = await _HttpRequest.PutAsync(CreateUrlByPutDocument(index, type, id), null, document, _LightningFormatter, null))
            {
                return httpResponse.IsHttpCreated();
            }
        }

        public async Task<T[]> SearchDocuments<T>(string index, object condition)
        {
            using (var httpResponse = await _HttpRequest.PostAsync(CreateUrlBySearchDocuments(index), null, condition, _LightningFormatter, null))
            {
                if (httpResponse.IsHttpOk())
                {
                    var response = await httpResponse.GetBodyAsync<SearchResponse<T>>(_LightningFormatter);
                    if (response.hits != null && response.hits.hits != null)
                    {
                        return response.hits.hits.Select(x => x._source).ToArray();
                    }
                }
            }
            return null;
        }

        private string CreateUrlByCreateIndex(string index)
        {
            return $"{HostUrl}/{index}";
        }

        private string CreateUrlByPutDocument(string index, string type, string id)
        {
            return $"{HostUrl}/{index}/{type}/{id}";
        }

        private string CreateUrlBySearchDocuments(string index)
        {
            return $"{HostUrl}/{index}/_search";
        }

        public class SearchResponse<T>
        {
            public SearchResult<T> hits { get; set; }
        }

        public class SearchResult<T>
        {
            public int total { get; set; }

            public SearchHit<T>[] hits { get; set; }
        }

        public class SearchHit<T>
        {
            public string _index { get; set; }

            public string _type { get; set; }

            public string _id { get; set; }

            public T _source { get; set; }
        }
    }
}