using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.RESTfulService
{
    [DI(typeof(IRESTfulServiceHandler), Lifetime = Lifetime.Singleton)]
    internal class RESTfulServiceHandler : IRESTfulServiceHandler
    {
        private readonly IRESTfulServiceFactory _Factory;

        private readonly IJsonFormatter _JsonFormatter;

        private readonly IXmlFormatter _XmlFormatter;

        private readonly ITextFormatter _TextFormatter;

        public RESTfulServiceHandler(
            IRESTfulServiceFactory factory,
            IJsonFormatter jsonFormatter,
            IXmlFormatter xmlFormatter,
            ITextFormatter textFormatter)
        {
            _Factory = factory;
            _Factory.Init(ContainerEntry.Container, new DefaultAssemblyLoader());

            _JsonFormatter = jsonFormatter;
            _XmlFormatter = xmlFormatter;
            _TextFormatter = textFormatter;
        }

        public IJsonFormatter JsonFormatter => _JsonFormatter;

        public IXmlFormatter XmlFormatter => _XmlFormatter;

        public ITextFormatter TextFormatter => _TextFormatter;

        public async Task ProcessRequest(ICallingContext context)
        {
            var callingContext = context as CallingContext;
            if (callingContext == null)
            {
                throw new Exception("calling context is null.");
            }

            var serviceContext = _Factory.GetService(
                callingContext.ServicePrefix,
                callingContext.ServiceName,
                callingContext.MethodName,
                callingContext.HttpVerb);

            var parameterValues = await GetParameterValuesAsync(
                serviceContext,
                callingContext.ContentType,
                callingContext.QueryString,
                callingContext.Context.Request.Body);

            var result = serviceContext.MethodInfo.Invoke(
                serviceContext.ServiceIntance,
                parameterValues);

            await SetResponse(
                GetResponseContentType(serviceContext, callingContext.Accept),
                result,
                callingContext.Context);
        }

        private async Task<object[]> GetParameterValuesAsync(ServiceContext context, string contentType, Dictionary<string, string> queryString, Stream stream)
        {
            var values = new object[context.MethodInfo.ParameterInfos.Length];

            for (int i = 0; i < context.MethodInfo.ParameterInfos.Length; i++)
            {
                var parameterInfo = context.MethodInfo.ParameterInfos[i];

                if (parameterInfo.Source == ParameterSource.QueryString)
                {
                    values[i] = parameterInfo.GetParameterValue(queryString);
                }
                else if (parameterInfo.Source == ParameterSource.Body)
                {
                    if (context.MethodInfo.RequestContentType != ContentType.Any)
                    {
                        if (context.MethodInfo.RequestContentType == ContentType.Json)
                        {
                            values[i] = await parameterInfo.GetParameterValueAsync(_JsonFormatter, stream);
                        }
                        else if (context.MethodInfo.RequestContentType == ContentType.Xml)
                        {
                            values[i] = await parameterInfo.GetParameterValueAsync(_XmlFormatter, stream);
                        }
                        else
                        {
                            values[i] = await parameterInfo.GetParameterValueAsync(_TextFormatter, stream);
                        }
                    }
                    else if (contentType.ContainsIgnoreCase("application/json"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_JsonFormatter, stream);
                    }
                    else if (contentType.ContainsIgnoreCase("application/xml"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_XmlFormatter, stream);
                    }
                    else
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_TextFormatter, stream);
                    }
                }
                else if (queryString.ContainsKey(parameterInfo.ParameterName))
                {
                    values[i] = parameterInfo.GetParameterValue(queryString);
                }
                else
                {
                    if (context.MethodInfo.RequestContentType != ContentType.Any)
                    {
                        if (context.MethodInfo.RequestContentType == ContentType.Json)
                        {
                            values[i] = await parameterInfo.GetParameterValueAsync(_JsonFormatter, stream);
                        }
                        else if (context.MethodInfo.RequestContentType == ContentType.Xml)
                        {
                            values[i] = await parameterInfo.GetParameterValueAsync(_XmlFormatter, stream);
                        }
                        else
                        {
                            values[i] = await parameterInfo.GetParameterValueAsync(_TextFormatter, stream);
                        }
                    }
                    else if (contentType.ContainsIgnoreCase("application/json"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_JsonFormatter, stream);
                    }
                    else if (contentType.ContainsIgnoreCase("application/xml"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_XmlFormatter, stream);
                    }
                    else
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_TextFormatter, stream);
                    }
                }
            }

            return values;
        }

        private ContentType GetResponseContentType(ServiceContext context, string accept)
        {
            if (context.MethodInfo.ResponseContentType == ContentType.Json)
            {
                return ContentType.Json;
            }
            else if (context.MethodInfo.ResponseContentType == ContentType.Xml)
            {
                return ContentType.Xml;
            }
            else if (context.MethodInfo.ResponseContentType == ContentType.Text)
            {
                return ContentType.Text;
            }
            else if (accept.ContainsIgnoreCase("application/json"))
            {
                return ContentType.Json;
            }
            else if (accept.ContainsIgnoreCase("application/xml"))
            {
                return ContentType.Xml;
            }
            else
            {
                return ContentType.Text;
            }
        }

        private async Task SetResponse(ContentType contentType, object result, HttpContext httpContext)
        {
            if (contentType == ContentType.Json)
            {
                httpContext.Response.ContentType = "application/json";

                await _JsonFormatter.WriteObjectAsync(result, httpContext.Response.Body);
            }
            else if (contentType == ContentType.Xml)
            {
                httpContext.Response.ContentType = "application/xml";

                var data = _XmlFormatter.WriteBytes(result);
                await httpContext.Response.Body.WriteAsync(data, 0, data.Length);
            }
            else
            {
                httpContext.Response.ContentType = "plain/text";

                await _TextFormatter.WriteObjectAsync(result, httpContext.Response.Body);
            }
        }
    }
}