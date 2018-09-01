using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Testing.Abstractions;
using System;
using System.Reflection;

namespace Guru.Testing.Implementation
{
    [Injectable(typeof(ITestManager), Lifetime.Singleton)]
    internal class DefaultTestManager : ITestManager
    {
        public bool TestModeEnabled { get; private set; }

        private readonly ITestProvider _TestProvider;

        private readonly ILightningFormatter _Formatter;

        public DefaultTestManager(ITestProvider testProvider, IJsonLightningFormatter jsonLightningFormatter)
        {
            _TestProvider = testProvider;
            _Formatter = jsonLightningFormatter;
        }

        public void EnableTestMode()
        {
            if (TestModeEnabled)
            {
                return;
            }

            TestModeEnabled = true;
        }

        public void DisableTestMode()
        {
            if (!TestModeEnabled)
            {
                return;
            }

            TestModeEnabled = false;
        }

        public void RunTest(string testClassName, string testMethodName)
        {
            var testMethod = _TestProvider.GetTestMethod(testClassName, testMethodName);
            if (testMethod == null)
            {
                return;
            }

            if (!testMethod.TestInputs.HasLength())
            {
                Console.WriteLine($"input of test method '{testMethodName}' not found.");
                return;
            }

            var instance = testMethod.TestClass.Prototype.IsAbstract ? null : DependencyContainer.Resolve(testMethod.TestClass.Prototype);
            foreach (var testInput in testMethod.TestInputs)
            {
                try
                {
                    var result = testMethod.Invoke(instance, testInput.InputValues);
                    if (result == null)
                    {
                        Console.WriteLine($"{testClassName}:{testMethodName} Passed.");
                        Console.WriteLine("Result: null");
                    }
                    else if (result.GetType() == typeof(string) || result.GetType().GetTypeInfo().IsValueType)
                    {
                        Console.WriteLine($"{testClassName}:{testMethodName} Passed.");
                        Console.WriteLine($"Result: {result}");
                    }
                    else
                    {
                        Console.WriteLine($"{testClassName}:{testMethodName} Passed.");
                        Console.WriteLine($"Result: {_Formatter.WriteObject(result)}");
                    }
                }
                catch (AssertFailureException e)
                {
                    Console.WriteLine($"test method '{testClassName}.{testMethodName}' assert failed.");
                    Console.WriteLine(e.GetInfo());
                }
                catch (Exception e)
                {
                    Console.WriteLine($"test method '{testClassName}.{testMethodName}' execute failed.");
                    Console.WriteLine(e.GetInfo());
                }
            }
        }
    }
}