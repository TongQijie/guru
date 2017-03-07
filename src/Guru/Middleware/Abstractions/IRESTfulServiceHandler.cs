using Guru.Formatter.Abstractions;

namespace Guru.Middleware.Abstractions
{
    public interface IRESTfulServiceHandler : IHttpHandler
    {
        IJsonFormatter JsonFormatter { get; }

        IXmlFormatter XmlFormatter { get; }

        ITextFormatter TextFormatter { get; }
    }
}