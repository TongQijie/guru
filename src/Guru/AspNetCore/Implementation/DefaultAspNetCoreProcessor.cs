using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;

namespace Guru.AspNetCore.Implementation
{
    [Injectable(typeof(IAspNetCoreProcessor), Lifetime.Singleton)]
    internal class DefaultAspNetCoreProcessor : IAspNetCoreProcessor
    {
        private readonly IResourceHandler _ResourceHandler;

        private readonly IApiHandler _ApiHandler;

        private readonly IApiMetadataHandler _ApiMetadataHandler;

        public DefaultAspNetCoreProcessor(IResourceHandler resourceHandler, IApiHandler apiHandler, IApiMetadataHandler apiMetadataHandler)
        {
            _ResourceHandler = resourceHandler;
            _ApiHandler = apiHandler;
            _ApiMetadataHandler = apiMetadataHandler;
        }

        public async Task Process(CallingContext context)
        {
            if (!context.RouteData.HasLength())
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HttpStatusCode,
                    Source = ContextParameterSource.Http,
                    Value = "400",
                });
                return;
            }

            var applicationConfiguration = context.ApplicationConfiguration;

            if (applicationConfiguration.Api != null &&
                applicationConfiguration.Api.Prefix.EqualsIgnoreCase(context.RouteData[0]))
            {
                // Api Request
                context.RouteData = context.RouteData.Subset(1);
                if (context.RouteData.HasLength() && context.RouteData[0].EqualsIgnoreCase("_Metadata"))
                {
                    context.RouteData = context.RouteData.Subset(1);
                    await _ApiMetadataHandler.ProcessRequest(context);
                }
                else
                {
                    await _ApiHandler.ProcessRequest(context);
                }
                return;
            }

            if (applicationConfiguration.Resource != null)
            {
                // Resource Request
                if (applicationConfiguration.Resource.Prefix.HasValue() &&
                    applicationConfiguration.Resource.Prefix.EqualsIgnoreCase(context.RouteData[0]))
                {
                    context.RouteData = context.RouteData.Subset(1);
                }

                await _ResourceHandler.ProcessRequest(context);
                return;
            }

            context.SetOutputParameter(new ContextParameter()
            {
                Name = CallingContextConstants.HttpStatusCode,
                Source = ContextParameterSource.Http,
                Value = "400",
            });
            return;
        }
    }
}