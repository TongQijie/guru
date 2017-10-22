using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation
{
    internal class ImplementationResolver : IImplementationResolver
    {
        public ImplementationResolver(IImplementationDecorator decorator)
        {
            Decorator = decorator;
        }

        public IImplementationDecorator Decorator { get; private set; }

        private object _SingletonObject = null;

        private object _Locker = new object();

        public object Resolve()
        {
            if (Decorator.Lifetime == Lifetime.Singleton)
            {
                if (_SingletonObject == null)
                {
                    lock (_Locker)
                    {
                        if (_SingletonObject == null)
                        {
                            _SingletonObject = CreateInstanceFactory.Instance.GetInstance(Decorator.ImplementationType);
                        }
                    }
                }

                return _SingletonObject;
            }
            else
            {
                return CreateInstanceFactory.Instance.GetInstance(Decorator.ImplementationType);
            }
        }
    }
}