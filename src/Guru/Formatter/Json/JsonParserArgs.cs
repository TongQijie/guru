using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    internal class JsonParserArgs
    {
        public IReaderStream Stream { get; set; }

        public JBase ExternalObject { get; set; }

        public JBase InternalObject { get; set; }

        public bool Handled { get; set; }
    }
}
