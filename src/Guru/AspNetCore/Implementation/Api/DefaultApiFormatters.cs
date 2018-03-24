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
            if (context.InputParameters.ContainsKey("formatter"))
            {
                var formatter = context.InputParameters.Get("formatter").Value;
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
            else if (context.InputParameters.ContainsKey("content-type"))
            {
                var contentType = context.InputParameters.Get("content-type").Value;
                if (contentType.ContainsIgnoreCase("json"))
                {
                    return _Json;
                }
                else if (contentType.ContainsIgnoreCase("xml"))
                {
                    return _Xml;
                }
                else if (contentType.ContainsIgnoreCase("text"))
                {
                    return _Text;
                }
            }
            else if (context.InputParameters.ContainsKey("accept"))
            {
                var accept = context.InputParameters.Get("accept").Value;
                if (accept.ContainsIgnoreCase("json"))
                {
                    return _Json;
                }
                else if (accept.ContainsIgnoreCase("xml"))
                {
                    return _Xml;
                }
                else if (accept.ContainsIgnoreCase("text"))
                {
                    return _Text;
                }
            }

            // default json api formatter
            return _Json;
        }
    }
}
