namespace Guru.DependencyInjection.Abstractions
{
    public interface IImplementationRegister
    {
        void Register(IContainerInstance instance);
    }
}