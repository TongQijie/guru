using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;

namespace Guru.Middleware
{
    public class AspNetCoreMiddleware
    {
        private readonly RequestDelegate _Next;

        private readonly IMiddlewareLifetime _Lifetime;

        private readonly IUriRewriteComponent _UriRewriteComponent;

        private readonly IDefaultUriComponent _DefaultUriComponent;

        private readonly IHttpHandlerComponent _HttpHandlerComponent;

        public AspNetCoreMiddleware(RequestDelegate next, IMiddlewareLifetime lifetime = null)
        {
            _Next = next;
            _Lifetime = lifetime;

            try
            {
                _UriRewriteComponent = ContainerManager.Default.Resolve<IUriRewriteComponent>();
                _DefaultUriComponent = ContainerManager.Default.Resolve<IDefaultUriComponent>();
                _HttpHandlerComponent = ContainerManager.Default.Resolve<IHttpHandlerComponent>();

                if (_Lifetime != null)
                {
                    _Lifetime.Startup();
                }

                Console.WriteLine("startup succeeded.");
            }
            catch (Exception e)
            {
                Console.WriteLine(new Logging.ExceptionWrapper(e).ToString());
                Console.WriteLine("startup failed.");
            }
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value.Trim('/');

            path = _UriRewriteComponent.Rewrite(path);

            if (!path.HasValue())
            {
                path = _DefaultUriComponent.Default();
            }

            await _HttpHandlerComponent.Process(path, context);
        }
    }
}