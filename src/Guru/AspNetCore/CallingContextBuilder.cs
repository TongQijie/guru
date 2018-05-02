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
                InputParameters = new DictionaryIgnoreCase<ContextParameter>(),
                OutputParameters = new DictionaryIgnoreCase<ContextParameter>(),
                ApplicationConfiguration = DependencyContainer.Resolve<IApplicationConfiguration>(),
            };

            if (httpContext.Request.Host != null)
            {
                context.InputParameters.AddOrUpdate("RequestHost", new ContextParameter()
                {
                    Name = "RequestHost",
                    Source = ContextParameterSource.Http,
                    Value = httpContext.Request.Host.Value,
                });
            }
            if (httpContext.Request.Path != null)
            {
                context.InputParameters.AddOrUpdate("RequestPath", new ContextParameter()
                {
                    Name = "RequestPath",
                    Source = ContextParameterSource.Http,
                    Value = httpContext.Request.Path.Value,
                });
            }
            if (!string.IsNullOrEmpty(httpContext.Request.Method))
            {
                context.InputParameters.AddOrUpdate("HttpMethod", new ContextParameter()
                {
                    Name = "HttpMethod",
                    Source = ContextParameterSource.Http,
                    Value = httpContext.Request.Method.ToUpper(),
                });
            }

            if (httpContext.Request.Query != null)
            {
                foreach (var kv in httpContext.Request.Query)
                {
                    if (!context.InputParameters.ContainsKey(kv.Key) && kv.Value.Count > 0)
                    {
                        context.InputParameters.AddOrUpdate(kv.Key, new ContextParameter()
                        {
                            Name = kv.Key,
                            Source = ContextParameterSource.QueryString,
                            Value = kv.Value[0],
                        });
                    }
                }
            }

            if (httpContext.Request.Headers != null)
            {
                foreach (var header in httpContext.Request.Headers)
                {
                    if (!context.InputParameters.ContainsKey(header.Key))
                    {
                        context.InputParameters.AddOrUpdate(header.Key, new ContextParameter()
                        {
                            Name = header.Key,
                            Source = ContextParameterSource.Header,
                            Value = string.Join(";", header.Value),
                        });
                    }
                }
            }

            if (httpContext.Request.HasFormContentType && httpContext.Request.Form != null)
            {
                foreach (var kv in httpContext.Request.Form)
                {
                    if (!context.InputParameters.ContainsKey(kv.Key) && kv.Value.Count > 0)
                    {
                        context.InputParameters.AddOrUpdate(kv.Key, new ContextParameter()
                        {
                            Name = kv.Key,
                            Source = ContextParameterSource.Form,
                            Value = kv.Value[0],
                        });
                    }
                }
            }
            else
            {
                context.InputStream = httpContext.Request.Body;
            }

            context.OutputStream = httpContext.Response.Body;
            context.SetOutputParameter = p =>
            {
                context.OutputParameters.AddOrUpdate(p.Name, p);
                if (p.Source == ContextParameterSource.Header)
                {
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