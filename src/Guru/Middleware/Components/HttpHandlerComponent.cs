using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Abstractions;
using Guru.Middleware.Configuration;

namespace Guru.Middleware.Components
{
    [DI(typeof(IHttpHandlerComponent), Lifetime = Lifetime.Singleton)]
    internal class HttpHandlerComponent : IHttpHandlerComponent
    {
        private readonly IStaticFileHandler _StaticFileHandler;

        private readonly IRESTfulServiceHandler _RESTfulServiceHandler;

        private readonly IErrorHandler _ErrorHandler;

        private readonly IFileManager _FileManager;

        public HttpHandlerComponent(
            IStaticFileHandler staticFileHandler,
            IRESTfulServiceHandler restfulServiceHandler,
            IErrorHandler errorHandler,
            IFileManager fileManager)
        {
            _StaticFileHandler = staticFileHandler;
            _RESTfulServiceHandler = restfulServiceHandler;
            _ErrorHandler = errorHandler;
            _FileManager = fileManager;
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
                var appConfig = _FileManager.Single<IApplicationConfiguration>();

                if (appConfig.ServicePrefixes.HasLength())
                {
                    if (appConfig.ServicePrefixes.Exists(x => x.EqualsIgnoreCase(fields[0])))
                    {
                        await _RESTfulServiceHandler.ProcessRequest(new RESTfulService.CallingContext(context, uri));
                    }
                    else
                    {
                        await _StaticFileHandler.ProcessRequest(new StaticFile.CallingContext(context, uri));
                    }
                }
                else
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
            }
            catch (Exception e)
            {
                await _ErrorHandler.ProcessRequest(new Errors.CallingContext(context, e));
            }
        }
    }
}