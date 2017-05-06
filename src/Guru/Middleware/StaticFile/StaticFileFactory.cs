using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.Middleware.Configuration;
using Guru.DependencyInjection.Attributes;

namespace Guru.Middleware.StaticFile
{
    [Injectable(typeof(IStaticFileFactory), Lifetime.Singleton)]
    internal class StaticFileFactory : IStaticFileFactory
    {
        public StaticFileContext GetStaticFile(string path, string resourceType)
        {
            var wwwRoot = "./wwwroot";

            var appConfig = ContainerManager.Default.Resolve<IApplicationConfiguration>();
            if (appConfig.WWWRoot.HasValue())
            {
                wwwRoot = appConfig.WWWRoot;
            }

            var fullPath = wwwRoot.FullPath() +  $"/{path.Trim('/')}";

            if (!appConfig.Resources.HasLength())
            {
                return null;
            }

            var resource = appConfig.Resources.FirstOrDefault(x => x.ResourceType.EqualsWith(resourceType) && (!x.AllowPath.HasValue() || x.AllowPath.FullPath().EqualsWithPath(fullPath.Folder())));
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