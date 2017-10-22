using System;

using Guru.DependencyInjection.Abstractions;
using Guru.DependencyInjection.Implementation;

namespace Guru.DependencyInjection
{
    public static class ContainerInstanceExtensionMethod
    {
        public static object Resolve(this IContainerInstance instance, Type abstraction)
        {
            return instance.GetImplementation(abstraction);
        }

        public static T Resolve<T>(this IContainerInstance instance)
        {
            return (T)instance.GetImplementation(typeof(T));
        }

        public static void RegisterSingleton(this IContainerInstance instance, Type abstraction, Type implementationType)
        {
            instance.Add(abstraction, new ImplementationResolver(new ImplementationDecorator(implementationType, Lifetime.Singleton, 0)));
        }
    }
}