using Microsoft.AspNetCore.Http;
using Guru.DependencyInjection;
using Guru.AspNetCore.Abstractions;
using Guru.ExtensionMethod;
using Guru.Foundation;

namespace Guru.AspNetCore
{
    internal class CallingContextBuilder
    {
        public static CallingContext Build(HttpContext httpContext)
        {
            var context = new CallingContext()
            {
                RequestHttpParameters = new IgnoreCaseKeyValues<string>(),
                RequestHeaderParameters = new IgnoreCaseKeyValues<string>(),
                InputParameters = new IgnoreCaseKeyValues<ContextParameter>(),
                ResponseHttpParameters = new IgnoreCaseKeyValues<string>(),
                ResponseHeaderParameters = new IgnoreCaseKeyValues<string>(),
                ApplicationConfiguration = DependencyContainer.Resolve<IApplicationConfiguration>(),
            };

            if (!string.IsNullOrEmpty(httpContext.Request.Method))
            {
                context.RequestHttpParameters.Add("Method", httpContext.Request.Method.ToUpper());
            }
            if (!string.IsNullOrEmpty(httpContext.Request.Scheme))
            {
                context.RequestHttpParameters.Add("Scheme", httpContext.Request.Scheme);
            }
            if (httpContext.Request.Host != null)
            {
                context.RequestHttpParameters.Add("Host", httpContext.Request.Host.Value);
            }
            if (httpContext.Request.Path != null)
            {
                context.RequestHttpParameters.Add("Path", httpContext.Request.Path.Value);
            }
            if (httpContext.Request.QueryString.HasValue)
            {
                context.RequestHttpParameters.Add("QueryString", httpContext.Request.QueryString.Value);
            }

            if (httpContext.Request.Headers != null)
            {
                foreach (var header in httpContext.Request.Headers)
                {
                    context.RequestHeaderParameters.Add(header.Key, string.Join(";", header.Value));
                }
            }

            if (httpContext.Request.Query != null)
            {
                foreach (var kv in httpContext.Request.Query)
                {
                    foreach (var value in kv.Value)
                    {
                        context.InputParameters.Add(kv.Key, new ContextParameter()
                        {
                            Name = kv.Key,
                            Source = ContextParameterSource.QueryString,
                            Value = value,
                        });
                    }
                }
            }

            if (httpContext.Request.HasFormContentType && httpContext.Request.Form != null)
            {
                foreach (var kv in httpContext.Request.Form)
                {
                    foreach (var value in kv.Value)
                    {
                        context.InputParameters.Add(kv.Key, new ContextParameter()
                        {
                            Name = kv.Key,
                            Source = ContextParameterSource.Form,
                            Value = value,
                        });
                    }
                }
            }

            context.InputStream = httpContext.Request.Body;
            context.OutputStream = httpContext.Response.Body;
            context.SetOutputParameter = p =>
            {
                if (p.Source == ContextParameterSource.Header)
                {
                    context.ResponseHeaderParameters.Add(p.Name, p.Value);
                    if (p.Name.EqualsIgnoreCase("Content-Type"))
                    {
                        httpContext.Response.ContentType = p.Value;
                    }
                    else if (p.Name.EqualsIgnoreCase("Content-Length"))
                    {
                        httpContext.Response.ContentLength = p.Value.ConvertTo<long>(-1);
                    }
                    else
                    {
                        httpContext.Response.Headers.Add(p.Name, p.Value);
                    }
                }
                else if (p.Source == ContextParameterSource.Http)
                {
                    context.ResponseHttpParameters.Add(p.Name, p.Value);
                    if (p.Name.EqualsIgnoreCase("StatusCode"))
                    {
                        if (int.TryParse(p.Value, out var statusCode))
                        {
                            httpContext.Response.StatusCode = statusCode;
                        }
                    }
                }
            };

            return context;
        }
    }
}