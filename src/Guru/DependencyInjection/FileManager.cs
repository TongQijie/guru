using Guru.ExtensionMethod;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    [DI(typeof(IFileManager), Lifetime = Lifetime.Singleton)]
    internal class FileManager : IFileManager
    {
        public T Single<T>()
        {
            return Container.Resolve<T>();
        }

        public T[] Many<T>()
        {
            return (Container.Resolve(typeof(T)) as object[]).Select(x => (T)x);
        }
    }
}