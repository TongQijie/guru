using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.Middleware.Configuration;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.StaticFile
{
    [DI(typeof(IStaticFileFactory), Lifetime = Lifetime.Singleton)]
    public class StaticFileFactory : IStaticFileFactory
    {
        public StaticFileContext GetStaticFile(string path, string resourceType)
        {
            var fullPath = $"./wwwroot/{path.Trim('/')}".FullPath();

            var resources = ContainerEntry.Resolve<IApplicationConfiguration>().Resources;
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