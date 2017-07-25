using System;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.AspNetCore.Abstractions;

namespace Guru.AspNetCore.Implementations
{
    [Injectable(typeof(IAspNetCoreComponent), Lifetime.Singleton)]
    public class DefaultAspNetCoreComponent : IAspNetCoreComponent
    {
        private readonly IAspNetCoreRouter _Router;

        private readonly IAspNetCoreProcessor _Processor;

        public DefaultAspNetCoreComponent(IAspNetCoreRouter router, IAspNetCoreProcessor processor)
        {
            _Router = router;
            _Processor = processor;
        }

        public async Task Process(CallingContext context)
        {
            _Router.GetRouteData(context);
            await _Processor.Process(context);
        }
    }
}