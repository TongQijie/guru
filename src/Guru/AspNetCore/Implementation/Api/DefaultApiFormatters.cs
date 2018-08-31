using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Implementation.Api.Formatter;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiFormatters), Lifetime.Singleton)]
    internal class DefaultApiFormatters : IApiFormatters
    {
        private readonly AbstractApiFormatter _Json;

        private readonly AbstractApiFormatter _Xml;

        private readonly AbstractApiFormatter _Text;

        public DefaultApiFormatters()
        {
            _Json = new JsonApiFormatter(DependencyContainer.ResolveOrDefault<ILightningFormatter, IJsonLightningFormatter>("JsonApiFormatter"));
            _Xml = new XmlApiFormatter(DependencyContainer.ResolveOrDefault<ILightningFormatter, IXmlLightningFormatter>("XmlApiFormatter"));
            _Text = new TextApiFormatter();
        }

        public AbstractApiFormatter Json => _Json;

        public AbstractApiFormatter Xml => _Xml;

        public AbstractApiFormatter Text => _Text;

        public AbstractApiFormatter Get(CallingContext context)
        {
            if (context.RequestHeaderParameters.ContainsKey("Formatter"))
            {
                var formatter = context.RequestHeaderParameters.GetValue("Formatter");
                if (formatter.ContainsIgnoreCase("json"))
                {
                    return _Json;
                }
                else if (formatter.ContainsIgnoreCase("xml"))
                {
                    return _Xml;
                }
                else if (formatter.ContainsIgnoreCase("text"))
                {
                    return _Text;
                }
            }
            else if (context.RequestHeaderParameters.ContainsKey("Content-Type"))
            {
                var contentType = context.RequestHeaderParameters.GetStringValue("Content-Type");
                if (contentType.ContainsIgnoreCase("application/json"))
                {
                    return _Json;
                }
                else if (contentType.ContainsIgnoreCase("application/xml"))
                {
                    return _Xml;
                }
                else if (contentType.ContainsIgnoreCase("plain/text"))
                {
                    return _Text;
                }
            }
            else if (context.RequestHeaderParameters.ContainsKey("Accept"))
            {
                var accept = context.RequestHeaderParameters.GetStringValue("Accept");
                if (accept.ContainsIgnoreCase("application/json"))
                {
                    return _Json;
                }
                else if (accept.ContainsIgnoreCase("application/xml"))
                {
                    return _Xml;
                }
                else if (accept.ContainsIgnoreCase("plain/text"))
                {
                    return _Text;
                }
            }

            // default json api formatter
            return _Json;
        }
    }
}
