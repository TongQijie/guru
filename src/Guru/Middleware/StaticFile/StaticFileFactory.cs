using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.Middleware.Configuration;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.StaticFile
{
    [DI(typeof(IStaticFileFactory), Lifetime = Lifetime.Singleton)]
    internal class StaticFileFactory : IStaticFileFactory
    {
        private readonly IFileManager _FileManager;

        public StaticFileFactory(IFileManager fileManager)
        {
            _FileManager = fileManager;
        }

        public StaticFileContext GetStaticFile(string path, string resourceType)
        {
            var wwwRoot = "./wwwroot";

            var appConfig = _FileManager.Single<IApplicationConfiguration>();
            if (appConfig.WWWRoot.HasValue())
            {
                wwwRoot = appConfig.WWWRoot;
            }

            var fullPath = wwwRoot.FullPath() +  $"/{path.Trim('/')}";

            var resources = Container.Resolve<IApplicationConfiguration>().Resources;
            if (!resources.HasLength())
            {
                return null;
            }

            var resource = resources.FirstOrDefault(x => x.ResourceType.EqualsWith(resourceType) && (!x.AllowPath.HasValue() || x.AllowPath.FullPath().EqualsWithPath(fullPath.Folder())));
            if (resource == null)
            {
                return null;
            }

            if (!fullPath.IsFile())
            {
                return null;
            }

            return new StaticFileContext()
            {
                Path = fullPath,
                ContentType = resource.ContentType,
            };
        }
    }
}