using System.IO;
using System.Threading.Tasks;

using Guru.Formatter.Xml;

namespace Guru.Formatter.Abstractions
{
    public interface IXmlParser
    {
        XBase Parse(Stream stream);

        Task<XBase> ParseAsync(Stream stream);
    }
}