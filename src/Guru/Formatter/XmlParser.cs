using System.IO;
using System.Threading.Tasks;

using Guru.Formatter.Xml;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Formatter
{
    [Injectable(typeof(IXmlParser), Lifetime.Singleton)]
    public class XmlParser : IXmlParser
    {
        public XBase Parse(Stream stream)
        {
            return Xml.XmlParser.ParseAsync(new Internal.BufferedReaderStream(stream, 8 * 1024)).GetAwaiter().GetResult();
        }

        public async Task<XBase> ParseAsync(Stream stream)
        {
            return await Xml.XmlParser.ParseAsync(new Internal.BufferedReaderStream(stream, 8 * 1024));
        }
    }
}
