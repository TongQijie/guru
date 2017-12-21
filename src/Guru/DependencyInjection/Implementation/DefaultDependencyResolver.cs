using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation
{
    internal class DefaultDependencyResolver : IDependencyResolver
    {
        public DefaultDependencyResolver(IDependencyDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public IDependencyDescriptor Descriptor { get; private set; }

        private object _SingletonObject = null;

        private object _Locker = new object();

        public object Resolve()
        {
            if (Descriptor.Lifetime == Lifetime.Singleton)
            {
                if (_SingletonObject == null)
                {
                    lock (_Locker)
                    {
                        if (_SingletonObject == null)
                        {
                            _SingletonObject = CreateInstanceFactory.Instance.GetInstance(Descriptor.ImplementationType);
                        }
                    }
                }

                return _SingletonObject;
            }
            else
            {
                return CreateInstanceFactory.Instance.GetInstance(Descriptor.ImplementationType);
            }
        }
    }
}