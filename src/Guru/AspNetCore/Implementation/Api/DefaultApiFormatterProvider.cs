using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Implementation.Api.Formatter;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiFormatterProvider), Lifetime.Singleton)]
    internal class DefaultApiFormatterProvider : IApiFormatterProvider
    {
        public DefaultApiFormatterProvider()
        {
            Json = new JsonApiFormatter(DependencyContainer.ResolveOrDefault<ILightningFormatter, IJsonLightningFormatter>("JsonApiFormatter"));
            Xml = new XmlApiFormatter(DependencyContainer.ResolveOrDefault<ILightningFormatter, IXmlLightningFormatter>("XmlApiFormatter"));
            Text = new TextApiFormatter();
        }

        public AbstractApiFormatter Json { get; private set; }

        public AbstractApiFormatter Xml { get; private set; }

        public AbstractApiFormatter Text { get; private set; }

        public AbstractApiFormatter Get(CallingContext context)
        {
            if (context.RequestHeaderParameters.ContainsKey("Formatter"))
            {
                var formatter = context.RequestHeaderParameters.GetValue("Formatter");
                if (formatter.ContainsIgnoreCase("json"))
                {
                    return Json;
                }
                else if (formatter.ContainsIgnoreCase("xml"))
                {
                    return Xml;
                }
                else if (formatter.ContainsIgnoreCase("text"))
                {
                    return Text;
                }
            }
            else if (context.RequestHeaderParameters.ContainsKey(CallingContextConstants.HeaderContentType))
            {
                var contentType = context.RequestHeaderParameters.GetStringValue(CallingContextConstants.HeaderContentType);
                if (contentType.ContainsIgnoreCase("application/json"))
                {
                    return Json;
                }
                else if (contentType.ContainsIgnoreCase("application/xml"))
                {
                    return Xml;
                }
                else if (contentType.ContainsIgnoreCase("plain/text"))
                {
                    return Text;
                }
            }
            else if (context.RequestHeaderParameters.ContainsKey("Accept"))
            {
                var accept = context.RequestHeaderParameters.GetStringValue("Accept");
                if (accept.ContainsIgnoreCase("application/json"))
                {
                    return Json;
                }
                else if (accept.ContainsIgnoreCase("application/xml"))
                {
                    return Xml;
                }
                else if (accept.ContainsIgnoreCase("plain/text"))
                {
                    return Text;
                }
            }

            // default json api formatter
            return Json;
        }
    }
}
