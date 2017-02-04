using System;

namespace Guru.DependencyInjection.Abstractions
{
    public interface IContainer
    {
        IConstructorInjectionFactory ConstructorInjectionFactory { get; }
        
        IContainer Use(IRegister register);
        
        bool CanInject(Type type);
        
        void Register(IResolver resolver);
        
        void RegisterTransient(Type abstraction, Type implementation, int priority);
        
        void RegisterTransient<TAbstraction, TImplementation>();

        void RegisterTransient<TAbstraction, TImplementation>(int priority);
        
        void RegisterSingleton(Type abstraction, Type implementation, int priority);
        
        void RegisterSingleton<TAbstraction, TImplementation>();

        void RegisterSingleton<TAbstraction, TImplementation>(int priority);
        
        object GetImplementation(Type type);
        
        T GetImplementation<T>();
    }
}