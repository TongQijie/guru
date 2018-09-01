using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Testing.Abstractions;
using Guru.Testing.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Guru.Testing.Implementation
{
    [Injectable(typeof(ITestProvider), Lifetime.Singleton)]
    internal class DefaultTestProvider : ITestProvider
    {
        private readonly Dictionary<string, ITestClass> _TestClasses = new Dictionary<string, ITestClass>();

        public DefaultTestProvider()
        {
            Init();
        }

        private void Init()
        {
            var assemblies = AssemblyLoader.Instance.GetAssemblies();

            if (!assemblies.HasLength())
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                foreach (var testClassType in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass && x.GetTypeInfo().IsDefined(typeof(TestClassAttribute), false)))
                {
                    var testClassAttribute = testClassType.GetTypeInfo().GetCustomAttribute<TestClassAttribute>();

                    var name = testClassAttribute.Name.HasValue() ? testClassAttribute.Name : testClassType.Name;

                    if (!testClassType.IsAbstract)
                    {
                        DependencyContainer.RegisterSingleton(testClassType, testClassType);
                    }

                    if (!_TestClasses.ContainsKey(name))
                    {
                        _TestClasses.Add(name, new DefaultTestClass(testClassType, name));
                    }
                }
            }
        }

        public ITestClass[] GetAllClasses()
        {
            var testClasses = new ITestClass[0];
            foreach (var testClass in _TestClasses)
            {
                testClasses = testClasses.Append(testClass.Value);
            }
            return testClasses;
        }

        public ITestMethod GetTestMethod(string testClassName, string testMethodName)
        {
            if (!_TestClasses.ContainsKey(testClassName))
            {
                Console.WriteLine($"test class '{testClassName}' not found.");
                return null;
            }

            var testClass = _TestClasses[testClassName];
            var testMethod = testClass.GetTestMethod(testMethodName);
            if (testMethod == null)
            {
                Console.WriteLine($"test method '{testMethodName}' not found.");
            }

            return testMethod;
        }
    }
}
