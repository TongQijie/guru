using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using System;
using System.IO;

namespace ConsoleApp.DependencyInjection
{
    public class TestRunner
    {
        public void Run()
        {
            var apple = ContainerEntry.Resolve<IAppleInterface>();
            Assert.IsTrue(apple.Banana != null);

            using (var outputStream = new FileStream("./dependencyinjection/cherry.json".FullPath(), FileMode.Create, FileAccess.Write))
            {
                var formatter = ContainerEntry.Resolve<IJsonFormatter>();

                var data = formatter.WriteBytes(new CherryClass() { A = 100 });

                outputStream.Write(data, 0, data.Length);
            }

            using (var outputStream = new FileStream("./dependencyinjection/durian_1.json".FullPath(), FileMode.Create, FileAccess.Write))
            {
                var formatter = ContainerEntry.Resolve<IJsonFormatter>();

                var data = formatter.WriteBytes(new DurianClass() { B = "hello, 1!" });

                outputStream.Write(data, 0, data.Length);
            }

            using (var outputStream = new FileStream("./dependencyinjection/durian_2.json".FullPath(), FileMode.Create, FileAccess.Write))
            {
                var formatter = ContainerEntry.Resolve<IJsonFormatter>();

                var data = formatter.WriteBytes(new DurianClass() { B = "hello, 2!" });

                outputStream.Write(data, 0, data.Length);
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