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
            _JsonFormatter = Container.Resolve<IJsonFormatter>();
        }

        public void Run()
        {
            var apple = Container.Resolve<IAppleInterface>();
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

            var cherry = Container.Resolve<ICherryInterface>();
            Assert.IsTrue(cherry.A == 100);

            var durians = (Container.Resolve(typeof(IDurianInterface)) as object[]).Select(x => x as IDurianInterface);
            Assert.IsTrue(durians.Length == 2 && durians[0].B == "hello, 1!" && durians[1].B == "hello, 2!");

            Console.WriteLine("hit 'esc' to stop loop...");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                cherry = Container.Resolve<ICherryInterface>();
                Console.WriteLine($"A: {cherry.A}");

                durians = (Container.Resolve(typeof(IDurianInterface)) as object[]).Select(x => x as IDurianInterface)
                    .Each(x => Console.WriteLine($"B: {x.B}"));
            }
        }
    }
}