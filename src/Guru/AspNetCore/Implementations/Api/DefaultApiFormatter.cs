using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Abstractions;

namespace Guru.AspNetCore.Implementations.Api
{
    [Injectable(typeof(IApiFormatter), Lifetime.Singleton)]
    public class DefaultApiFormatter : IApiFormatter
    {
        private readonly IFormatter _Json;

        private readonly IFormatter _Xml;

        private readonly IFormatter _Text;

        public DefaultApiFormatter(IJsonFormatter json, IXmlFormatter xml, ITextFormatter text)
        {
            _Json = json;
            _Xml = xml;
            _Text = text;
        }

        public IFormatter GetFormatter(string name)
        {
            switch (name)
            {
                case "json": return _Json;
                case "xml": return _Xml;
                case "text": return _Text;
                default: return null;
            }
        }
    }
}