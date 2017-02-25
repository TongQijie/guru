using System;
using System.Diagnostics;

using Guru.DependencyInjection;

namespace ConsoleApp
{
    public class Program
    {
        static Program()
        {
            ContainerEntry.Init(new DefaultAssemblyLoader());
        }

        public static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            new DependencyInjection.TestRunner().Run();

            // new Formatter.TestRunner().Run();

            // new Network.TestRunner().Run();

            // new Network.UnsplashCrawler().Run();

            // new EntityFramework.TestRunner().Run();

            stopwatch.Stop();
            Console.WriteLine($"test done. cost: {stopwatch.Elapsed.TotalMilliseconds}ms.");
        }
    }
}