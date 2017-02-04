using System;

namespace Guru.DynamicProxy
{
    public interface IDynamicProxyGenerator
    {
        object CreateProxyObject(Type baseClass, object[] parameters = null);

        T CreateProxyObject<T>(object[] parameters = null);

        Type CreateProxyType(Type baseClass);
    }
}
