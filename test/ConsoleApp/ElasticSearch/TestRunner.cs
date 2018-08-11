using Guru.DependencyInjection;
using Guru.ElasticSearch.Abstractions;
using Guru.Network.Abstractions;
using System;
using Guru.ExtensionMethod;
using System.Threading.Tasks;

namespace ConsoleApp.ElasticSearch
{
    public class TestRunner
    {
        public async Task Run()
        {
            var provider = DependencyContainer.Resolve<IElasticSearchProvider>();

            provider.Configure("http://mysql:9200");

            await provider.PutDocument("test", "type", Guid.NewGuid().ToString(), new Document()
            {
                Title = "ThisTestTitle",
                Level = 1,
                Timestamp = DateTime.Now.Timestamp(),
            });

            var docs = await provider.SearchDocuments<Document>("test", "{\"query\":{\"match_all\":{}}}");
        }

        public class Document
        {
            public string Title { get; set; }

            public int Level { get; set; }

            public long Timestamp { get; set; }
        }
    }
}
