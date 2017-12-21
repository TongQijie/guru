namespace Guru.DependencyInjection.Abstractions
{
    internal interface IDependencyResolver
    {
        IDependencyDescriptor Descriptor { get; }

        object Resolve();
    }
}