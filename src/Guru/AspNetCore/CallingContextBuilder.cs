using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Guru.DependencyInjection;
using Guru.AspNetCore.Abstractions;
using Guru.ExtensionMethod;

namespace Guru.AspNetCore
{
    internal class CallingContextBuilder
    {
        public static CallingContext Build(HttpContext httpContext)
        {
            var context = new CallingContext()
            {
                InputParameters = new Dictionary<string, ContextParameter>(),
                ApplicationConfiguration = ContainerManager.Default.Resolve<IApplicationConfiguration>(),
            };

            context.InputParameters.Add("requestpath", new ContextParameter()
            {
                Name = "requestpath",
                Source = ContextParameterSource.Http,
                Value = httpContext.Request.Path.Value,
            });
            context.InputParameters.Add("httpmethod", new ContextParameter()
            {
                Name = "httpmethod",
                Source = ContextParameterSource.Http,
                Value = httpContext.Request.Method.ToUpper(),
            });

            foreach (var kv in httpContext.Request.Query)
            {
                if (!context.InputParameters.ContainsKey(kv.Key.ToLower()) && kv.Value.Count > 0)
                {
                    context.InputParameters.Add(kv.Key.ToLower(), new ContextParameter()
                    {
                        Name = kv.Key.ToLower(),
                        Source = ContextParameterSource.QueryString,
                        Value = kv.Value[0],
                    });
                }
            }

            foreach (var header in httpContext.Request.Headers)
            {
                if (!context.InputParameters.ContainsKey(header.Key.ToLower()))
                {
                    context.InputParameters.Add(header.Key.ToLower(), new ContextParameter()
                    {
                        Name = header.Key.ToLower(),
                        Source = ContextParameterSource.Header,
                        Value = string.Join(";", header.Value),
                    });
                }
            }

            if (httpContext.Request.HasFormContentType)
            {
                foreach (var kv in httpContext.Request.Form)
                {
                    if (!context.InputParameters.ContainsKey(kv.Key.ToLower()) && kv.Value.Count > 0)
                    {
                        context.InputParameters.Add(kv.Key.ToLower(), new ContextParameter()
                        {
                            Name = kv.Key.ToLower(),
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
                if (p.Source == ContextParameterSource.Header)
                {
                    if (p.Name.EqualsIgnoreCase("Content-Type"))
                    {
                        httpContext.Response.ContentType = p.Value;
                    }
                    else
                    {
                        httpContext.Response.Headers.Add(p.Name, p.Value);
                    }
                }
            };

            return context;
        }
    }
}