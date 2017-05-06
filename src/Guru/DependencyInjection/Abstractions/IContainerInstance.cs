namespace Guru.DependencyInjection.Abstractions
{
    public interface IContainerInstance
    {
        void Add(object key, IImplementationResolver resolver);

        bool Exists(object key);

        object GetImplementation(object key);

        IContainerInstance Register(IImplementationRegister register);
    }
}