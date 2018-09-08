using Guru.AspNetCore.Attributes;
using Guru.ExtensionMethod;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Guru.AspNetCore.Implementation.Api.Definition
{
    internal class ApiMethodDefinition
    {
        private readonly bool _IsAsyncMethod;

        private readonly Type[] _ReturnTypeGenericParameters;

        private readonly HandlingBeforeAttribute _HandlingBefore;

        private readonly HandlingAfterAttribute _HandlingAfter;

        public ApiMethodDefinition(MethodInfo prototype, string methodName, bool defaultMethod)
        {
            Prototype = prototype;
            MethodName = methodName;
            DefaultMethod = defaultMethod;
            _HandlingBefore = prototype.GetCustomAttribute<HandlingBeforeAttribute>();
            _HandlingAfter = prototype.GetCustomAttribute<HandlingAfterAttribute>();

            _IsAsyncMethod = prototype.IsDefined(typeof(AsyncStateMachineAttribute));
            if (_IsAsyncMethod)
            {
                if (!prototype.ReturnType.GetTypeInfo().IsGenericType)
                {
                    _ReturnTypeGenericParameters = new Type[1] { typeof(void) };
                }
                else
                {
                    _ReturnTypeGenericParameters = prototype.ReturnType.GetGenericArguments();
                }
            }
        }

        public object Invoke(object instance, params object[] parameters)
        {
            var id = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

            if (_HandlingBefore != null)
            {
                var rst = _HandlingBefore.Handle(id, ReturnType, parameters);
                if (rst == null)
                {
                    return null;
                }

                if (!rst.Succeeded)
                {
                    return rst.ResultObject;
                }
            }

            object result;
            if (!_IsAsyncMethod)
            {
                result = Prototype.Invoke(instance, parameters);
            }
            else
            {
                result = _HandleAsyncMethod.MakeGenericMethod(_ReturnTypeGenericParameters).Invoke(this, new object[] { Prototype.Invoke(instance, parameters) });
            }

            if (_HandlingAfter != null)
            {
                var rst = _HandlingAfter.Handle(id, ReturnType, result);
                if (rst == null)
                {
                    return null;
                }

                if (!rst.Succeeded)
                {
                    return rst.ResultObject;
                }
            }

            return result;
        }

        static ApiMethodDefinition()
        {
            _HandleAsyncMethod = typeof(ApiMethodDefinition).GetMethod("HandleAsync", BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static readonly MethodInfo _HandleAsyncMethod;

        private static T HandleAsync<T>(Task task)
        {
            return ((Task<T>)task).GetAwaiter().GetResult();
        }

        public string MethodName { get; set; }

        public bool DefaultMethod { get; set; }

        public ApiParameterDefinition[] Parameters { get; set; }

        public MethodInfo Prototype { get; set; }

        public Type ReturnType
        {
            get
            {
                if (_IsAsyncMethod)
                {
                    if (_ReturnTypeGenericParameters.HasLength())
                    {
                        return _ReturnTypeGenericParameters[0];
                    }
                    else
                    {
                        return typeof(void);
                    }
                }
                else
                {
                    return Prototype.ReturnType;
                }
            }
        }
    }
}