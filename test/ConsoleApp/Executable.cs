using System;
using System.Diagnostics;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using Guru.Utils;
using System.Threading;
using Guru.Logging.Abstractions;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Executable : IConsoleExecutable
    {
        private readonly ILogger _Logger;

        public Executable(IFileLogger fileLogger)
        {
            _Logger = fileLogger;
        }

        public int Run(string[] args)
        {
            //new DependencyInjection.TestRunner().Run();

            //new Formatter.TestRunner().Run();

            //new Network.TestRunner().Run();

            // new Network.UnsplashCrawler().Run();

            // new EntityFramework.TestRunner().Run();

            new Cache.TestRunner().Run();

            // new Mq.TestRunner().Run();

            //new Jobs.TestRunner().Run();

            //new Markdown.TestRunner().Run();
            
            return 0;
        }

        public Task<int> RunAsync(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}