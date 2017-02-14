using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.Middleware.Configuration;

namespace Guru.Middleware
{
    public class AspNetCoreMiddleware
    {
        private readonly RequestDelegate _Next;

        public AspNetCoreMiddleware(RequestDelegate next)
        {
            _Next = next;

            var loader = new DefaultAssemblyLoader();
            ContainerEntry.Init(loader);
        }

        public async Task Invoke(HttpContext context)
        {
            // code in this method MUST NOT throw exception.
            // so, please be cautious!
            
            ICallingContext callingContext;
            
            var fields = context.Request.Path.Value.SplitByChar('/');

            // apply rewrite rules
            var rewrites = ContainerEntry.Resolve<IApplicationConfiguration>().Rewrites;
            if (rewrites.HasLength())
            {
                var url = string.Join("/", fields);
                foreach (var item in rewrites)
                {
                    if (Regex.IsMatch(url, item.Pattern, RegexOptions.IgnoreCase))
                    {
                        if (item.Mode == RewriteMode.Override)
                        {
                            fields = item.Value.SplitByChar('/');
                        }
                        else if (item.Mode == RewriteMode.Replace)
                        {
                            fields = Regex.Replace(url, item.Pattern, item.Value).SplitByChar('/');
                        }
                    }
                }
            }

            // check if fields is empty. if empty, apply default route.
            if (!fields.HasLength())
            {
                var routes = ContainerEntry.Resolve<IApplicationConfiguration>().Routes;
                if (routes.HasLength())
                {
                    var config = routes.FirstOrDefault(x => x.Key.EqualsWith("default"));
                    if (config != null && config.Value.HasValue())
                    {
                        fields = fields.Append(config.Value.SplitByChar('/'));
                    }
                }
            }

            if (fields.HasLength() && fields[fields.Length - 1].Contains("."))
            {
                var lastField = fields[fields.Length - 1];
                var dotIndex = lastField.LastIndexOf('.');
                
                callingContext = new StaticFile.CallingContext(context, string.Join("/", fields), (dotIndex < lastField.Length - 1) ? lastField.Substring(dotIndex + 1) : string.Empty);

                await ContainerEntry.Resolve<IStaticFileHandler>().ProcessRequest(callingContext);
            }
            else
            {
                callingContext = new RESTfulService.CallingContext(context, fields.Length > 0 ? fields[0] : "", fields.Length > 1 ? fields[1] : "");
                
                await ContainerEntry.Resolve<IRESTfulServiceHandler>().ProcessRequest(callingContext);
            }
        }
    }
}