using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.StaticFile
{
    [DI(typeof(IStaticFileHandler), Lifetime = Lifetime.Singleton)]
    internal class StaticFileHandler : IStaticFileHandler
    {
        private readonly IStaticFileFactory _Factory;

        public StaticFileHandler(IStaticFileFactory factory)
        {
            _Factory = factory;
        }

        public async Task ProcessRequest(ICallingContext context)
        {
            var callingContext = context as CallingContext;
            if (callingContext == null)
            {
                throw new Exception("calling context is null.");
            }

            var staticFileContext = _Factory.GetStaticFile(callingContext.Path, callingContext.ResourceType);
            if (staticFileContext == null)
            {
                context.Context.Response.StatusCode = 404;
                return;
            }

            await SetResponse(staticFileContext, context.Context);
        }

        private async Task SetResponse(StaticFileContext context, HttpContext httpContext)
        {
            httpContext.Response.ContentType = context.ContentType;

            using (var inputStream = new FileStream(context.Path, FileMode.Open, FileAccess.Read))
            {
                var count = 0;
                var buffer = new byte[16 * 1024];
                while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await httpContext.Response.Body.WriteAsync(buffer, 0, count);
                }
            }
        }
    }
}
