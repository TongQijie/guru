using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    internal class JsonObjectParseArgs
    {
        public IReaderStream Stream { get; set; }

        public JsonObject ExternalObject { get; set; }

        public JsonObject InternalObject { get; set; }

        public bool Handled { get; set; }
    }
}
