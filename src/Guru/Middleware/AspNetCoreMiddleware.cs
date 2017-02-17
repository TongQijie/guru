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

        private readonly IUriRewriteComponent _UriRewriteComponent;

        private readonly IDefaultUriComponent _DefaultUriComponent;

        private readonly IHttpHandlerComponent _HttpHandlerComponent;

        public AspNetCoreMiddleware(RequestDelegate next)
        {
            _Next = next;

            var loader = new DefaultAssemblyLoader();
            ContainerEntry.Init(loader);

            _UriRewriteComponent = ContainerEntry.Resolve<IUriRewriteComponent>();
            _DefaultUriComponent = ContainerEntry.Resolve<IDefaultUriComponent>();
            _HttpHandlerComponent = ContainerEntry.Resolve<IHttpHandlerComponent>();
        }

        public async Task Invoke(HttpContext context)
        {
            var path = _UriRewriteComponent.Rewrite(context.Request.Path.Value);

            if (!path.HasValue())
            {
                path = _DefaultUriComponent.Default();
            }

            await _HttpHandlerComponent.Process(path, context);
        }
    }
}