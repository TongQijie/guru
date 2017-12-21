namespace Guru.DependencyInjection.Abstractions
{
    internal interface IDependencyRegister
    {
        IContainerInstance Register(IContainerInstance instance);
    }
}