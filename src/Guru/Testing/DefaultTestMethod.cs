using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
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

        public ITestClass TestClass { get; private set; }

        public string Name { get; private set; }

        public DefaultTestMethod(ITestClass testClass, MethodInfo prototype, string name)
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
            TestClass = testClass;
            Name = name;
            var parameterTypes = prototype.GetParameters().Select(x => x.ParameterType);
            foreach (var testInputAttribute in prototype.GetCustomAttributes<TestInputAttribute>())
            {
                if (testInputAttribute.InputValues.HasLength() && parameterTypes.HasLength() && parameterTypes.Length == testInputAttribute.InputValues.Length)
                {
                    var parameterValues = new object[testInputAttribute.InputValues.Length];
                    for (int i = 0; i < testInputAttribute.InputValues.Length; i++)
                    {
                        if (parameterTypes[i].IsClass && parameterTypes[i] != typeof(string) && testInputAttribute.InputValues[i].GetType() == typeof(string))
                        {
                            parameterValues[i] = DependencyContainer.Resolve<IJsonLightningFormatter>().ReadObject(parameterTypes[i], testInputAttribute.InputValues[i].ToString());
                        }
                        else
                        {
                            parameterValues[i] = testInputAttribute.InputValues[i].ConvertTo(parameterTypes[i]);
                        }
                    }
                    TestInputs = TestInputs.Append(new DefaultTestInput(parameterValues));
                }
            }
        }

        public object Invoke(object instance, params object[] parameters)
        {
            try
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
            catch (Exception e)
            {
                if (e.InnerException.GetType() == typeof(AssertFailureException))
                {
                    throw new AssertFailureException($"test method '{Name}' assert failed.", e.InnerException);
                }
                throw;
            }
        }
    }
}
