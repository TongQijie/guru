using Guru.Formatter.Internal;

namespace Guru.Formatter.Xml
{
    internal class XmlParserArgs
    {
        public IReaderStream Stream { get; set; }

        public XBase ExternalObject { get; set; }

        public XBase InternalObject { get; set; }

        public bool Handled { get; set; }
    }
}
