using Guru.DependencyInjection;

namespace Guru.Utils
{
    public static class StaticFileUtils
    {
        public static T Get<T>()
        {
            return DependencyContainer.Resolve<T>();
        }
    }
}