using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware
{
    public interface IMiddlewareLifetime
    {
        void Startup(IContainer container);
    }
}