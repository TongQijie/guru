using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;

namespace Guru.AspNetCore.Implementations
{
    [Injectable(typeof(IAspNetCoreProcessor), Lifetime.Singleton)]
    public class DefaultAspNetCoreProcessor : IAspNetCoreProcessor
    {
        private readonly IResourceHandler _ResourceHandler;

        private readonly IApiHandler _ApiHandler;

        public DefaultAspNetCoreProcessor(IResourceHandler resourceHandler, IApiHandler apiHandler)
        {
            _ResourceHandler = resourceHandler;
            _ApiHandler = apiHandler;
        }

        public async Task Process(CallingContext context)
        {
            if (!context.RouteData.HasLength())
            {
                // 404
                return;
            }

            var applicationConfiguration = context.ApplicationConfiguration;

            if (applicationConfiguration.Resource != null && 
                applicationConfiguration.Resource.Prefix.EqualsIgnoreCase(context.RouteData[0]))
            {
                // Resource Request
                context.RouteData = context.RouteData.Subset(1);
                await _ResourceHandler.ProcessRequest(context);
                return;
            }
            
            if (applicationConfiguration.Api != null &&
                applicationConfiguration.Api.Prefix.EqualsIgnoreCase(context.RouteData[0]))
            {
                // Api Request
                context.RouteData = context.RouteData.Subset(1);
                await _ApiHandler.ProcessRequest(context);
                return;
            }

            // 404
            return;
        }
    }
}