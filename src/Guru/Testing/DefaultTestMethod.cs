using Guru.ExtensionMethod;
using Guru.Testing.Abstractions;
using Guru.Testing.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Guru.Testing
{
    internal class DefaultTestMethod : ITestMethod
    {
        static DefaultTestMethod()
        {
            _HandleAsyncMethod = typeof(DefaultTestMethod).GetMethod("HandleAsync", BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static readonly MethodInfo _HandleAsyncMethod;

        private static T HandleAsync<T>(Task task)
        {
            return ((Task<T>)task).GetAwaiter().GetResult();
        }

        private readonly MethodInfo _Prototype;

        private readonly bool _IsAsyncMethod;

        private readonly Type[] _ReturnTypeGenericParameters;

        public ITestInput[] TestInputs { get; private set; }

        public string Name { get; private set; }

        public DefaultTestMethod(MethodInfo prototype, string name)
        {
            _Prototype = prototype;
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
            Name = name;
            foreach (var testInputAttribute in prototype.GetCustomAttributes<TestInputAttribute>())
            {
                TestInputs = TestInputs.Append(new DefaultTestInput(testInputAttribute.InputValues));
            }
        }

        public object Invoke(object instance, params object[] parameters)
        {
            if (!_IsAsyncMethod)
            {
                return _Prototype.Invoke(instance, parameters);
            }
            else
            {
                return _HandleAsyncMethod.MakeGenericMethod(_ReturnTypeGenericParameters).Invoke(this, new object[] { _Prototype.Invoke(instance, parameters) });
            }
        }
    }
}
