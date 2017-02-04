using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Guru.Logging;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.StaticFile
{
    [DI(typeof(IStaticFileHandler), Lifetime = Lifetime.Singleton)]
    public class StaticFileHandler : IStaticFileHandler
    {
        private readonly IStaticFileFactory _Factory;

        private readonly IFileLogger _FileLogger;
        
        private readonly byte[] _ErrorBytes;
        
        public StaticFileHandler(IStaticFileFactory factory, IFileLogger fileLogger)
        {
            _Factory = factory;
            _FileLogger = fileLogger;
            
            _ErrorBytes = Encoding.UTF8.GetBytes("sorry, i had trouble to process this request.");
        }
        
        public async Task ProcessRequest(ICallingContext context)
        {
            var callingContext = context as CallingContext;
            
            try
            {
                var staticFileContext = _Factory.GetStaticFile(callingContext.Path, callingContext.ResourceType);
                if (staticFileContext == null)
                {
                    context.Context.Response.StatusCode = 404;
                    return;
                }

                await SetResponse(staticFileContext, context.Context);
            }
            catch (Exception e)
            {
                _FileLogger.LogEvent("RESTfulServiceHandler", Severity.Error, "failed to process request.", e);

                context.Context.Response.StatusCode = 500;
                await callingContext.Context.Response.Body.WriteAsync(_ErrorBytes, 0, _ErrorBytes.Length);
            }
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
