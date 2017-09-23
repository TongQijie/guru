using System.IO;
using System.Threading.Tasks;

using Guru.Formatter.Json;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Formatter
{
    [Injectable(typeof(IJsonParser), Lifetime.Singleton)]
    public class JsonParser : IJsonParser
    {
        public JBase Parse(Stream stream)
        {
            var args = new JsonParserArgs()
            {
                Stream = new Internal.BufferedReaderStream(stream, 8 * 1024),
            };

            Json.JsonParser.Parse(args);

            return args.InternalObject;
        }

        public async Task<JBase> ParseAsync(Stream stream)
        {
            var args = new JsonParserArgs()
            {
                Stream = new Internal.BufferedReaderStream(stream, 8 * 1024),
            };

            await Json.JsonParser.ParseAsync(args);

            return args.InternalObject;
        }
    }
}
