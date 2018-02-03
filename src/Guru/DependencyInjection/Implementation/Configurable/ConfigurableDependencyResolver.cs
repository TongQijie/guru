using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation.Configurable
{
    internal class ConfigurableDependencyResolver : IDependencyResolver
    {
        public ConfigurableDependencyResolver(IDependencyDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public IDependencyDescriptor Descriptor { get; set; }

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
                            _SingletonObject = Instantiate(Descriptor as ConfigurableDependencyDescriptor);
                        }
                    }
                }

                return _SingletonObject;
            }
            else
            {
                return Instantiate(Descriptor as ConfigurableDependencyDescriptor);
            }
        }

        private object Instantiate(ConfigurableDependencyDescriptor descriptor)
        {
            if (descriptor == null || descriptor.ImplementationType == null)
            {
                return null;
            }

            var instance = CreateInstanceFactory.Instance.GetInstance(Descriptor.ImplementationType);
            if (instance == null)
            {
                return null;
            }

            if (descriptor.PropertySetters == null || descriptor.PropertySetters.Count == 0)
            {
                return instance;
            }

            foreach (var setter in descriptor.PropertySetters)
            {
                setter.PropertyInfo.SetValue(instance, setter.PropertyValue);
            }

            return instance;
        }
    }
}
