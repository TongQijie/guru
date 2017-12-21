namespace Guru.DependencyInjection.Abstractions
{
    internal interface IContainerInstance
    {
        void Add(object key, IDependencyResolver resolver);

        bool Exists(object key);

        object GetImplementation(object key);

        IContainerInstance Register(IDependencyRegister register);
    }
}