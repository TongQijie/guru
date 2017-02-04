using Guru.Middleware.StaticFile;

namespace Guru.Middleware.Abstractions
{
    public interface IStaticFileFactory
    {
        StaticFileContext GetStaticFile(string path, string resourceType);
    }
}