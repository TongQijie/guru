using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Guru.Logging;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.RESTfulService
{
    [DI(typeof(IRESTfulServiceHandler), Lifetime = Lifetime.Singleton)]
    public class RESTfulServiceHandler : IRESTfulServiceHandler
    {
        private readonly IRESTfulServiceFactory _Factory;

        private readonly IJsonFormatter _JsonFormatter;

        private readonly IXmlFormatter _XmlFormatter;

        private readonly IFileLogger _FileLogger;
        
        private readonly byte[] _ErrorBytes;

        public RESTfulServiceHandler(
            IRESTfulServiceFactory factory,
            IJsonFormatter jsonFormatter,
            IXmlFormatter xmlFormatter,
            IFileLogger fileLogger)
        {
            _Factory = factory;
            _Factory.Init(ContainerEntry.Container, new DefaultAssemblyLoader());

            _JsonFormatter = jsonFormatter;
            _XmlFormatter = xmlFormatter;
            _FileLogger = fileLogger;

            _ErrorBytes = Encoding.UTF8.GetBytes("sorry, i had trouble to process this request.");
        }

        public async Task ProcessRequest(ICallingContext context)
        {
            var callingContext = context as CallingContext;

            try
            {
                var serviceContext = _Factory.GetService(callingContext.ServiceName, callingContext.MethodName, callingContext.HttpVerb);

                var parameterValues = await GetParameterValuesAsync(serviceContext, callingContext.ContentType, callingContext.QueryString, callingContext.Context.Request.Body);

                var result = serviceContext.MethodInfo.Invoke(serviceContext.ServiceIntance, parameterValues);

                await SetResponse(GetResponseContentType(serviceContext, callingContext.Accept), result, callingContext.Context);
            }
            catch (Exception e)
            {
                _FileLogger.LogEvent("RESTfulServiceHandler", Severity.Error, "failed to process request.", e);

                context.Context.Response.StatusCode = 500;
                await callingContext.Context.Response.Body.WriteAsync(_ErrorBytes, 0, _ErrorBytes.Length);
            }
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
                            values[i] = await parameterInfo.GetParameterValueAsync(null, stream);
                        }
                    }
                    else if (contentType.Contains("application/json"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_JsonFormatter, stream);
                    }
                    else if (contentType.Contains("application/xml"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_XmlFormatter, stream);
                    }
                    else
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(null, stream);
                    }
                }
                else if (queryString.ContainsKey(parameterInfo.Name))
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
                            values[i] = await parameterInfo.GetParameterValueAsync(null, stream);
                        }
                    }
                    else if (contentType.Contains("application/json"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_JsonFormatter, stream);
                    }
                    else if (contentType.Contains("application/xml"))
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(_XmlFormatter, stream);
                    }
                    else
                    {
                        values[i] = await parameterInfo.GetParameterValueAsync(null, stream);
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
            else if (accept.Contains("application/json"))
            {
                return ContentType.Json;
            }
            else if (accept.Contains("application/xml"))
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

                var data = _JsonFormatter.WriteBytes(result);
                await httpContext.Response.Body.WriteAsync(data, 0, data.Length);
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

                var data = Encoding.UTF8.GetBytes(result.ToString());
                await httpContext.Response.Body.WriteAsync(data, 0, data.Length);
            }
        }
    }
}