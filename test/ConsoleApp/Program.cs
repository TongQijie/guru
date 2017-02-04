using Guru.DependencyInjection;
using System;
using System.Diagnostics;

namespace ConsoleApp
{
    public class Program
    {
        static Program()
        {
            try
            {
                ContainerEntry.Init(new DefaultAssemblyLoader());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            // init dependency injection
            //ContainerEntry.Init(new DefaultAssemblyLoader());
        }

        public static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //new DependencyInjection.TestRunner().Run();

            //new Formatter.TestRunner().Run();

            //new Network.TestRunner().Run();

            new EntityFramework.TestRunner().Run();

            stopwatch.Stop();
            Console.WriteLine($"test done. cost: {stopwatch.Elapsed.TotalMilliseconds}ms.");
        }
    }
}