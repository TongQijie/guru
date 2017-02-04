using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace ConsoleApp.DependencyInjection
{
    [DI(typeof(IBananaInterface), Lifetime = Lifetime.Transient)]
    public class BananaClass : IBananaInterface
    {
    }
}
