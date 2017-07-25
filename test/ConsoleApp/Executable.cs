using System;
using System.Diagnostics;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;

namespace ConsoleApp
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Executable : IConsoleExecutable
    {
        public void Run(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //new DependencyInjection.TestRunner().Run();

            //new Formatter.TestRunner().Run();

            new Network.TestRunner().Run();

            // new Network.UnsplashCrawler().Run();

            // new EntityFramework.TestRunner().Run();

            // new Cache.TestRunner().Run();

            // new Mq.TestRunner().Run();

            //new Jobs.TestRunner().Run();

            //new Markdown.TestRunner().Run();

            stopwatch.Stop();
            //Console.WriteLine($"test done. cost: {stopwatch.Elapsed.TotalMilliseconds}ms.");
        }
    }
}