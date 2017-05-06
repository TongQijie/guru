using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.Middleware.Configuration;
using Guru.DependencyInjection.Attributes;

namespace Guru.Middleware.Components
{
    [Injectable(typeof(IDefaultUriComponent), Lifetime.Singleton)]
    internal class DefaultUriComponent : IDefaultUriComponent
    {
        public string Default()
        {
            var routes = ContainerManager.Default.Resolve<IApplicationConfiguration>().Routes;
            if (routes.HasLength())
            {
                var config = routes.FirstOrDefault(x => x.Key.EqualsWith("default"));
                if (config != null && config.Value.HasValue())
                {
                    return config.Value;
                }
            }

            return string.Empty;
        }
    }
}