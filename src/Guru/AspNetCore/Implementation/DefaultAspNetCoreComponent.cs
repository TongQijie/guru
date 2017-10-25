using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.AspNetCore.Implementation
{
    [Injectable(typeof(IAspNetCoreComponent), Lifetime.Singleton)]
    internal class DefaultAspNetCoreComponent : IAspNetCoreComponent
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