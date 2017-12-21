using System;
using StackExchange.Redis;
using Guru.Cache.Abstractions;
using Guru.DependencyInjection;
using System.Threading;
using Guru.Cache.Delegates;

namespace ConsoleApp.Cache
{
    public class TestRunner
    {
        private readonly IMemoryCacheProvider _MemoryCacheProvider;

        public TestRunner()
        {
            _MemoryCacheProvider = DependencyContainer.Resolve<IMemoryCacheProvider>();
        }

        public void Run()
        {
            Console.WriteLine(_MemoryCacheProvider.Get<int>("Integer"));

            Console.WriteLine(_MemoryCacheProvider.Set<int>("Integer", 100, TimeSpan.FromSeconds(6)));

            Console.WriteLine(_MemoryCacheProvider.Get<int>("Integer"));

            Thread.Sleep(3000);

            SetDelegate<int> setDelegate = x => 200;

            Console.WriteLine(_MemoryCacheProvider.GetOrSet<int>("Integer", setDelegate, TimeSpan.FromSeconds(10)));

            Thread.Sleep(4000);

            Console.WriteLine(_MemoryCacheProvider.GetOrSet<int>("Integer", setDelegate, TimeSpan.FromSeconds(10)));

            Console.WriteLine(_MemoryCacheProvider.Remove("Integer"));

            Console.WriteLine(_MemoryCacheProvider.Get<int>("Integer"));
        }
    }
}