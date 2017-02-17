using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Guru.ExtensionMethod;
using Guru.Middleware.Abstractions;

namespace Guru.Middleware.Components
{
    public class HttpHandlerComponent : IHttpHandlerComponent
    {
        private readonly IStaticFileHandler _StaticFileHandler;

        private readonly IRESTfulServiceHandler _RESTfulServiceHandler;

        public HttpHandlerComponent(
            IStaticFileHandler staticFileHandler, 
            IRESTfulServiceHandler restfulServiceHandler)
        {
            _StaticFileHandler = staticFileHandler;
            _RESTfulServiceHandler = restfulServiceHandler;
        }

        public async Task Process(string uri, HttpContext context)
        {
            var fields = uri.SplitByChar('/');

            if (fields.HasLength() && fields[fields.Length - 1].Contains("."))
            {
                var callingContext = new StaticFile.CallingContext(context, uri);

                await _StaticFileHandler.ProcessRequest(callingContext);
            }
            else
            {
                var callingContext = new RESTfulService.CallingContext(context, uri);
                
                await _RESTfulServiceHandler.ProcessRequest(callingContext);
            }
        }
    }
}