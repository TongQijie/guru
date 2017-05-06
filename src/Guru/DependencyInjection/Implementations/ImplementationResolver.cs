using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementations
{
    internal class ImplementationResolver : IImplementationResolver
    {
        public ImplementationResolver(IImplementationDecorator decorator)
        {
            Decorator = decorator;
        }

        public IImplementationDecorator Decorator { get; private set; }

        private object _SingletonObject = null;

        public object Resolve()
        {
            if (Decorator.Lifetime == Lifetime.Singleton && _SingletonObject != null)
            {
                return _SingletonObject;
            }

            var obj = CreateInstanceFactory.Instance.GetInstance(Decorator.ImplementationType);

            if (Decorator.Lifetime == Lifetime.Singleton)
            {
                _SingletonObject = obj;
            }

            return obj;
        }
    }
}