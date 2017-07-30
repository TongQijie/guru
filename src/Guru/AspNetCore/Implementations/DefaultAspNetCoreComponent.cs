using System.Text;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.AspNetCore.Implementations
{
    [Injectable(typeof(IAspNetCoreComponent), Lifetime.Singleton)]
    public class DefaultAspNetCoreComponent : IAspNetCoreComponent
    {
        private readonly IAspNetCoreRouter _Router;

        private readonly IAspNetCoreProcessor _Processor;

        private readonly ILogger _Logger;

        public DefaultAspNetCoreComponent(IAspNetCoreRouter router, IAspNetCoreProcessor processor, IFileLogger logger)
        {
            _Router = router;
            _Processor = processor;
            _Logger = logger;
        }

        public async Task Process(CallingContext context)
        {
            System.Console.WriteLine($"Component enter");

            LogRequest(context);
            _Router.GetRouteData(context);
            await _Processor.Process(context);
        }

        private void LogRequest(CallingContext context)
        {
            var stringBuilder = new StringBuilder();
            foreach (var parameter in context.InputParameters)
            {
                stringBuilder.AppendLine($"[{parameter.Value.Source.ToString()}] {parameter.Key}={parameter.Value.Value}");
            }
            _Logger.LogEvent("DefaultAspNetCoreComponent", Severity.Information, stringBuilder.ToString());
        }
    }
}