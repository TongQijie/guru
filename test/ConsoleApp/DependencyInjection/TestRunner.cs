using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using System;
using System.IO;

namespace ConsoleApp.DependencyInjection
{
    public class TestRunner
    {
        private readonly IJsonFormatter _JsonFormatter;

        public TestRunner()
        {
            _JsonFormatter = ContainerEntry.Resolve<IJsonFormatter>();
        }

        public void Run()
        {
            var apple = ContainerEntry.Resolve<IAppleInterface>();
            Assert.IsTrue(apple.Banana != null);

            "./dependencyinjection".EnsureFolder();

            using (var outputStream = new FileStream("./dependencyinjection/cherry.json".FullPath(), FileMode.Create, FileAccess.Write))
            {
                _JsonFormatter.WriteObject(new CherryClass() { A = 100 }, outputStream);
            }

            using (var outputStream = new FileStream("./dependencyinjection/durian_1.json".FullPath(), FileMode.Create, FileAccess.Write))
            {
                _JsonFormatter.WriteObject(new DurianClass() { B = "hello, 1!" }, outputStream);
            }

            using (var outputStream = new FileStream("./dependencyinjection/durian_2.json".FullPath(), FileMode.Create, FileAccess.Write))
            {
                _JsonFormatter.WriteObject(new DurianClass() { B = "hello, 2!" }, outputStream);
            }

            var cherry = ContainerEntry.Resolve<ICherryInterface>();
            Assert.IsTrue(cherry.A == 100);

            var durians = (ContainerEntry.Resolve(typeof(IDurianInterface)) as object[]).Select(x => x as IDurianInterface);
            Assert.IsTrue(durians.Length == 2 && durians[0].B == "hello, 1!" && durians[1].B == "hello, 2!");

            Console.WriteLine("hit 'esc' to stop loop...");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                cherry = ContainerEntry.Resolve<ICherryInterface>();
                Console.WriteLine($"A: {cherry.A}");

                durians = (ContainerEntry.Resolve(typeof(IDurianInterface)) as object[]).Select(x => x as IDurianInterface)
                    .Each(x => Console.WriteLine($"B: {x.B}"));
            }
        }
    }
}