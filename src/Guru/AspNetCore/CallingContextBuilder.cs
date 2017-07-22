using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Guru.AspNetCore
{
    internal class CallingContextBuilder
    {
        public static CallingContext Build(HttpContext httpContext)
        {
            var context = new CallingContext()
            {
                InputParameters = new Dictionary<string, string>(),
            };

            context.InputParameters.Add("requestpath", httpContext.Request.Path.Value);
            context.InputParameters.Add("httpmethod", httpContext.Request.Method.ToUpper());

            foreach (var kv in httpContext.Request.Query)
            {
                if (!context.InputParameters.ContainsKey(kv.Key.ToLower()) && kv.Value.Count > 0)
                {
                    context.InputParameters.Add(kv.Key.ToLower(), kv.Value[0]);
                }
            }

            foreach (var header in httpContext.Request.Headers)
            {
                if (!context.InputParameters.ContainsKey(header.Key.ToLower()))
                {
                    context.InputParameters.Add(header.Key.ToLower(), string.Join(";", header.Value));
                }
            }

            if (httpContext.Request.HasFormContentType)
            {
                foreach (var kv in httpContext.Request.Form)
                {
                    if (!context.InputParameters.ContainsKey(kv.Key.ToLower()) && kv.Value.Count > 0)
                    {
                        context.InputParameters.Add(kv.Key.ToLower(), kv.Value[0]);
                    }
                }
            }
            else
            {
                context.InputStream = httpContext.Request.Body;
            }

            return context;
        }
    }
}