namespace Guru.DependencyInjection.Abstractions
{
    public interface IImplementationResolver
    {
        IImplementationDecorator Decorator { get; }

        object Resolve();
    }
}