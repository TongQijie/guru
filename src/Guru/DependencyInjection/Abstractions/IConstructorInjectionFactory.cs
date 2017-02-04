using System;

namespace Guru.DependencyInjection.Abstractions
{
    public interface IConstructorInjectionFactory
    {
        object GetInstance(Type type);
    }
}