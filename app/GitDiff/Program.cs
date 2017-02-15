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

            var git = ContainerEntry.Resolve<IGit>();
            git.LocalDirectory = "d:\\git\\shoppingservice";

            var commits = git.GetTotalCommits(
                DateTime.Parse("2016-12-28T14:30:09+08:00"),
                DateTime.Parse("2017-01-14T13:40:41+08:00"),
                "Project_13574"
            );

            var diff = ContainerEntry.Resolve<IDiff>();
            diff.Execute(commits);

            stopwatch.Stop();
            Console.WriteLine($"test done. cost: {stopwatch.Elapsed.TotalMilliseconds}ms.");
        }
    }
}