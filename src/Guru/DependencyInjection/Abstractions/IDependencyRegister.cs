using System.Reflection;

namespace Guru.DependencyInjection.Abstractions
{
    internal interface IDependencyRegister
    {
        IContainerInstance Register(IContainerInstance instance);

        IContainerInstance Register(IContainerInstance instance, Assembly assembly);
    }
}