using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DynamicProxy
{
    [DI(typeof(IDynamicProxyGenerator), Lifetime = Lifetime.Singleton)]
    public class DynamicProxyGenerator : IDynamicProxyGenerator
    {
        private AssemblyBuilder _AssemblyBuilder = null;

        private ModuleBuilder _ModuleBuilder = null;

        private const string DynamicModuleName = "DynamicProxyModule";

        private const string DynamicAssemblyName = "DynamicProxy.dll";

        private static DynamicProxyGenerator _Instance;

        public static DynamicProxyGenerator Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DynamicProxyGenerator();
                }

                return _Instance;
            }
        }

        public DynamicProxyGenerator()
        {
            _AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName() { Name = DynamicAssemblyName }, AssemblyBuilderAccess.Run);
            _ModuleBuilder = _AssemblyBuilder.DefineDynamicModule(DynamicModuleName);

            // DEBUG
            //_AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName() { Name = DynamicAssemblyName }, AssemblyBuilderAccess.RunAndSave);
            //_ModuleBuilder = _AssemblyBuilder.DefineDynamicModule(DynamicModuleName, DynamicAssemblyName);
        }

        private ConcurrentDictionary<Type, Type> _GeneratedProxyTypes = null;

        public ConcurrentDictionary<Type, Type> GeneratedProxyTypes
        {
            get { return _GeneratedProxyTypes ?? (_GeneratedProxyTypes = new ConcurrentDictionary<Type, Type>()); }
        }

        public object CreateProxyObject(Type baseClass, object[] parameters = null)
        {
            return Activator.CreateInstance(CreateProxyType(baseClass), parameters);
        }

        public T CreateProxyObject<T>(object[] parameters = null)
        {
            return (T)CreateProxyObject(typeof(T), parameters);
        }

        public Type CreateProxyType(Type baseClass)
        {
            if (!baseClass.GetTypeInfo().IsClass)
            {
                // TODO: throw
            }

            if (!GeneratedProxyTypes.ContainsKey(baseClass))
            {
                GeneratedProxyTypes[baseClass] = GenerateProxyType(baseClass);
            }

            return GeneratedProxyTypes[baseClass];
        }

        private Type GenerateProxyType(Type baseClass)
        {
            var typeBuilder = _ModuleBuilder.DefineType(string.Format("{0}.{1}", DynamicModuleName, baseClass.Name), TypeAttributes.Public, baseClass);

            foreach (var constructorMethod in baseClass.GetTypeInfo().GetConstructors())
            {
                var parameterInfos = constructorMethod.GetParameters();

                var builder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterInfos.Select(x => x.ParameterType).ToArray());

                var il = builder.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);

                for (var i = 0; i < parameterInfos.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, i + 1);
                }

                il.Emit(OpCodes.Call, constructorMethod);

                il.Emit(OpCodes.Ret);
            }

            foreach (var instanceMethod in baseClass.GetTypeInfo().GetMethods())
            {
                var attribute = instanceMethod.GetCustomAttribute<MethodInterceptorAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var parameterInfos = instanceMethod.GetParameters();
                var hasParameters = parameterInfos.Length > 0;
                var hasReturnValue = instanceMethod.ReturnType != typeof(void);

                var methodBuilder = typeBuilder.DefineMethod(instanceMethod.Name, instanceMethod.Attributes,
                    instanceMethod.ReturnType, parameterInfos.Select(x => x.ParameterType).ToArray());

                var il = methodBuilder.GetILGenerator();

                il.DeclareLocal(typeof(InvocationBase));
                il.DeclareLocal(typeof(IInterceptor));
                il.DeclareLocal(typeof(bool));
                if (hasReturnValue)
                {
                    il.DeclareLocal(instanceMethod.ReturnType);
                }

                // new InvocationBase()
                il.Emit(OpCodes.Newobj, typeof(InvocationBase).GetTypeInfo().GetConstructor(new Type[] { }));
                il.Emit(OpCodes.Stloc_0);

                // get type of baseClass
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldtoken, baseClass);
                il.Emit(OpCodes.Call, typeof(Type).GetTypeInfo().GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) }));

                // set invocationBase.TargetType
                il.Emit(OpCodes.Callvirt, typeof(InvocationBase).GetTypeInfo().GetMethod("set_TargetType", new Type[] { typeof(Type) }));

                il.Emit(OpCodes.Ldloc_0);

                // define array: object[]
                il.Emit(OpCodes.Ldc_I4, parameterInfos.Length);
                il.Emit(OpCodes.Newarr, typeof(object));

                if (parameterInfos.Length > 0)
                {
                    // set elements
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Ldc_I4, i);
                        il.Emit(OpCodes.Ldarg, i + 1);
                        il.Emit(OpCodes.Box, parameterInfos[i].ParameterType);
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                }

                // set InvocationBase.ParameterValues
                il.Emit(OpCodes.Callvirt, typeof(InvocationBase).GetTypeInfo().GetMethod("set_ParameterValues", new Type[] { typeof(object[]) }));

                // get type of baseClass
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldtoken, baseClass);
                il.Emit(OpCodes.Call, typeof(Type).GetTypeInfo().GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) }));

                // load Method Name
                il.Emit(OpCodes.Ldstr, instanceMethod.Name);

                // define array Type[parameterInfos.Length]
                il.Emit(OpCodes.Ldc_I4, parameterInfos.Length);
                il.Emit(OpCodes.Newarr, typeof(Type));

                if (hasParameters)
                {
                    // set elements
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Ldc_I4, i);
                        il.Emit(OpCodes.Ldtoken, parameterInfos[i].ParameterType);
                        il.Emit(OpCodes.Call, typeof(Type).GetTypeInfo().GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) }));
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                }

                // call method: GetMethod(string, Type[])
                il.Emit(OpCodes.Call, typeof(Type).GetTypeInfo().GetMethod("GetMethod", new Type[] { typeof(string), typeof(Type[]) }));

                // set InvocationBase.MethodInfo
                il.Emit(OpCodes.Callvirt, typeof(InvocationBase).GetTypeInfo().GetMethod("set_MethodInfo", new Type[] { typeof(MethodInfo) }));

                // get interceptor type
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldtoken, attribute.Type);
                il.Emit(OpCodes.Call, typeof(Type).GetTypeInfo().GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) }));

                // set invocationBase.InterceptorType
                il.Emit(OpCodes.Callvirt, typeof(InvocationBase).GetTypeInfo().GetMethod("set_InterceptorType", new Type[] { typeof(Type) }));

                //
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Callvirt, typeof(InvocationBase).GetTypeInfo().GetMethod("get_InterceptorType", new Type[] { }));
                //il.Emit(OpCodes.Call, typeof(DependencyInjector).GetNonGenericMethod("GetObject", new Type[] { typeof(Type) }));
                il.Emit(OpCodes.Isinst, typeof(IInterceptor));

                //
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Stloc_2);

                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Brfalse_S, (byte)0x0C); // TODO

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Callvirt, typeof(InvocationBase).GetTypeInfo().GetMethod("get_InterceptorType", new Type[] { }));
                il.Emit(OpCodes.Newobj, typeof(InterceptorNotFoundException).GetTypeInfo().GetConstructor(new Type[] { typeof(Type) }));
                il.Emit(OpCodes.Throw);

                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Callvirt, typeof(IInterceptor).GetTypeInfo().GetMethod("Intercept", new Type[] { typeof(IInvocation) }));

                if (hasReturnValue)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Callvirt, typeof(InvocationBase).GetTypeInfo().GetMethod("get_ReturnValue", new Type[] { }));
                    il.Emit(OpCodes.Unbox_Any, instanceMethod.ReturnType);
                    il.Emit(OpCodes.Stloc_3);
                    il.Emit(OpCodes.Ldloc_3);
                }

                il.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(methodBuilder, baseClass.GetTypeInfo().GetMethod(instanceMethod.Name, parameterInfos.Select(x => x.ParameterType).ToArray()));
            }

            var targetType = typeBuilder.CreateTypeInfo().AsType();

            // DEBUG
            //_AssemblyBuilder.Save(DynamicAssemblyName);

            return targetType;
        }
    }
}
