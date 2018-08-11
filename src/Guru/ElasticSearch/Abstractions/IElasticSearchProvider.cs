using System.Threading.Tasks;

namespace Guru.ElasticSearch.Abstractions
{
    public interface IElasticSearchProvider
    {
        void Configure(string hostUrl);

        Task<bool> CreateIndex(string index);

        Task<bool> PutDocument(string index, string type, string id, object document);

        Task<T[]> SearchDocuments<T>(string index, object condition);
    }
}