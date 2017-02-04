using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    public class ConstructorInjectionFactory : IConstructorInjectionFactory
    {
        private static readonly ConcurrentDictionary<Type, Type[]> _ConstructorParameterTypes = new ConcurrentDictionary<Type, Type[]>();
        
        private static readonly ConcurrentDictionary<Type, Func<object[], object>> _ConstructorDelegates = new ConcurrentDictionary<Type, Func<object[], object>>();
        
        private readonly IContainer _Container;
        
        public ConstructorInjectionFactory(IContainer container)
        {
            _Container = container;
        }
        
        public object GetInstance(Type type)
        {
            var parameterTypes = GetConstructorParameterTypes(type);

            object[] paramaters = parameterTypes.Select(t => _Container.GetImplementation(t));
            Func<object[], object> constructorDelegate;
            _ConstructorDelegates.TryGetValue(type, out constructorDelegate);
            if (constructorDelegate == null)
            {
                throw new Exception($"i cannot find a constructor of type '{type.FullName}'.");
            }
            
            return constructorDelegate(paramaters);
        }
        
        private Type[] GetConstructorParameterTypes(Type type)
        {
            return _ConstructorParameterTypes.GetOrAdd(type, ConstructorParameterTypesFactory);
        }

        private Type[] ConstructorParameterTypesFactory(Type type)
        {
            Type[] result = null;

            foreach (var ctor in type.GetConstructors())
            {
                Type[] ctorParamsTypes = ctor.GetParameters().Select(param => param.ParameterType);
                
                if (ctorParamsTypes.Exists(x => !_Container.CanInject(x)))
                {
                    continue;
                }

                result = ctorParamsTypes;
                _ConstructorDelegates.GetOrAdd(type, ConstructorDelegateFactory(ctor));
            }
            
            if (result == null)
            {
                throw new Exception($"i cannot locate a constructor of type '{type.FullName}' that all of which parameters can inject.");
            }

            return result;
        }
        
        private Func<object[], object> ConstructorDelegateFactory(ConstructorInfo constructorInfo)
        {
            // Target: (object)new T((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = constructorInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreate = Expression.New(constructorInfo, parameterExpressions);

            // (object)new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreateCast = Expression.Convert(instanceCreate, typeof(object));

            var lambda = Expression.Lambda<Func<object[], object>>(instanceCreateCast, parametersParameter);

            return lambda.Compile();
        }
    }
}