using Guru.ExtensionMethod;
using Guru.Testing.Abstractions;
using Guru.Testing.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Guru.Testing
{
    internal class DefaultTestClass : ITestClass
    {
        private readonly Dictionary<string, ITestMethod> _TestMethods = new Dictionary<string, ITestMethod>();

        public Type Prototype { get; private set; }

        public string Name { get; private set; }

        public DefaultTestClass(Type prototype, string name)
        {
            Prototype = prototype;
            Name = name;

            foreach (var methodInfo in prototype.GetMethods())
            {
                var methodAttribute = methodInfo.GetCustomAttribute<TestMethodAttribute>();
                if (methodAttribute == null)
                {
                    continue;
                }

                var methodName = methodAttribute.Name.HasValue() ? methodAttribute.Name : methodInfo.Name;

                if (!_TestMethods.ContainsKey(methodName))
                {
                    _TestMethods.Add(methodName, new DefaultTestMethod(this, methodInfo, methodName));
                }
            }
        }

        public ITestMethod[] GetAllMethods()
        {
            var testMethods = new ITestMethod[0];
            foreach (var testMethod in _TestMethods)
            {
                testMethods = testMethods.Append(testMethod.Value);
            }
            return testMethods; 
        }

        public ITestMethod GetTestMethod(string name)
        {
            if (_TestMethods.ContainsKey(name))
            {
                return _TestMethods[name];
            }
            return null;
        }
    }
}
