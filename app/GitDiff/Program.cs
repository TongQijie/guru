using System;
using System.Diagnostics;

using Guru.DependencyInjection;

namespace GitDiff
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
            
            var diff = ContainerEntry.Resolve<IDiff>();
            diff.Execute();

            stopwatch.Stop();
            Console.WriteLine($"done. total cost: {stopwatch.Elapsed.TotalMilliseconds}ms.");
        }
    }
}