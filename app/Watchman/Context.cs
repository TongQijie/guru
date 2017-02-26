using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace Watchman
{
    [DI(typeof(IContext), Lifetime = Lifetime.Singleton)]
    public class Context : IContext
    {
        public string Source { get; set; }

        public string Target { get; set; }
    }
}