using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.Components
{
    [DI(typeof(IHttpHandlerComponent), Lifetime = Lifetime.Singleton)]
    internal class HttpHandlerComponent : IHttpHandlerComponent
    {
        private readonly IStaticFileHandler _StaticFileHandler;

        private readonly IRESTfulServiceHandler _RESTfulServiceHandler;

        private readonly IErrorHandler _ErrorHandler;

        public HttpHandlerComponent(
            IStaticFileHandler staticFileHandler,
            IRESTfulServiceHandler restfulServiceHandler,
            IErrorHandler errorHandler)
        {
            _StaticFileHandler = staticFileHandler;
            _RESTfulServiceHandler = restfulServiceHandler;
            _ErrorHandler = errorHandler;
        }

        public async Task Process(string uri, HttpContext context)
        {
            var fields = uri.SplitByChar('/');

            if (!fields.HasLength())
            {
                await _ErrorHandler.ProcessRequest(new Errors.CallingContext(context, null));
                return;
            }

            try
            {
                if (fields[fields.Length - 1].Contains("."))
                {
                    await _StaticFileHandler.ProcessRequest(new StaticFile.CallingContext(context, uri));
                }
                else
                {
                    await _RESTfulServiceHandler.ProcessRequest(new RESTfulService.CallingContext(context, uri));
                }
            }
            catch (Exception e)
            {
                await _ErrorHandler.ProcessRequest(new Errors.CallingContext(context, e));
            }
        }
    }
}