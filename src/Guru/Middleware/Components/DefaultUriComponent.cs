using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.Middleware.Configuration;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.Components
{
    [DI(typeof(IDefaultUriComponent), Lifetime = Lifetime.Singleton)]
    public class DefaultUriComponent : IDefaultUriComponent
    {
        private readonly IFileManager _FileManager;

        public DefaultUriComponent(IFileManager fileManager)
        {
            _FileManager = fileManager;
        }

        public string Default()
        {
            var routes = _FileManager.Single<IApplicationConfiguration>().Routes;
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