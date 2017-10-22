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

        private readonly IRequestLogger _Logger;

        public DefaultAspNetCoreComponent(IAspNetCoreRouter router, IAspNetCoreProcessor processor, IRequestLogger logger)
        {
            _Router = router;
            _Processor = processor;
            _Logger = logger;
        }

        public bool NeedLog { get; set; }

        public async Task Process(CallingContext context)
        {
            if (NeedLog) { _Logger.LogEvent(nameof(DefaultAspNetCoreComponent), context); }
            _Router.GetRouteData(context);
            await _Processor.Process(context);
        }
    }
}