using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;

namespace ConsoleApp.DependencyInjection
{
    [Injectable(typeof(IBananaInterface), Lifetime.Transient)]
    public class BananaClass : IBananaInterface
    {
    }
}
