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

                    DependencyContainer.RegisterSingleton(testClassType, testClassType);

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

        public object Invoke(string testClassName, string testMethodName, object[] parameters)
        {
            if (!_TestClasses.ContainsKey(testClassName))
            {
                throw new Exception($"test class '{testClassName}' not found.");
            }

            var testClass = _TestClasses[testClassName];
            var testMethod = testClass.GetTestMethod(testMethodName);
            if (testMethod == null)
            {
                throw new Exception($"test method '{testMethodName}' not found.");
            }

            return testMethod.Invoke(DependencyContainer.Resolve(testClass.Prototype), parameters);
        }

        public void Run(string testClassName, string testMethodName)
        {
            if (!_TestClasses.ContainsKey(testClassName))
            {
                throw new Exception($"test class '{testClassName}' not found.");
            }

            var testClass = _TestClasses[testClassName];
            var testMethod = testClass.GetTestMethod(testMethodName);
            if (testMethod == null)
            {
                throw new Exception($"test method '{testMethodName}' not found.");
            }

            if (!testMethod.TestInputs.HasLength())
            {
                return;
            }

            var instance = DependencyContainer.Resolve(testClass.Prototype);
            foreach (var testInput in testMethod.TestInputs)
            {
                testMethod.Invoke(instance, testInput.InputValues);
            }
        }
    }
}
