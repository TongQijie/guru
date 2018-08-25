using System;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable;
using Guru.Executable.Abstractions;
using Guru.ExtensionMethod;
using Guru.Testing.Abstractions;

namespace Guru.Testing
{
    [Injectable(typeof(IStartup), Lifetime.Singleton)]
    internal class TestingStartup : IStartup
    {
        private readonly ITestProvider _TestProvider;

        public TestingStartup(ITestProvider testProvider)
        {
            _TestProvider = testProvider;
        }

        public async Task<int> RunAsync(CommandLineArgs args)
        {
            var allTestMethods = new ITestMethod[0];
            var testClasses = _TestProvider.GetAllClasses();
            if (!testClasses.HasLength())
            {
                Console.WriteLine("there is no test classes found.");
                return await Task.FromResult(0);
            }
            foreach (var testClass in testClasses)
            {
                Console.WriteLine($"{testClass.Name}");
                var testMethods = testClass.GetAllMethods();
                if (!testMethods.HasLength())
                {
                    continue;
                }
                foreach (var testMethod in testMethods)
                {
                    allTestMethods = allTestMethods.Append(testMethod);
                    Console.WriteLine($"\t{allTestMethods.Length}:{testMethod.Name}");
                }
            }
            Console.Write("Input index: ");
            var input = "";
            while ((input = Console.ReadLine()) != "quit")
            {
                var index = input.ConvertTo(0);
                if (index <= 0 || index >= allTestMethods.Length)
                {
                    Console.WriteLine("index is invalid.");
                }
                else
                {
                    var testMethod = allTestMethods[index];
                    var testClass = testClasses.FirstOrDefault(x => x.GetAllMethods().Exists(y => y == testMethod));
                    _TestProvider.Run(testClass.Name, allTestMethods[index].Name);
                }
                Console.Write("Input index: ");
            }
            return await Task.FromResult(0);
        }
    }
}