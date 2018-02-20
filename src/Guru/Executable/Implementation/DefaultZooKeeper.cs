using System;
using System.Collections.Concurrent;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;

namespace Guru.Executable.Implementation
{
    [Injectable(typeof(IZooKeeper), Lifetime.Singleton)]
    public class DefaultZooKeeper : IZooKeeper
    {
        private ConcurrentBag<IDisposable> _Animals = new ConcurrentBag<IDisposable>();

        public void Add(IDisposable disposable)
        {
            if (!_Animals.TryPeek(out var d))
            {
                _Animals.Add(disposable);
            }
        }

        public void RemoveAll()
        {
            foreach (var animal in _Animals)
            {
                animal.Dispose();
            }

            _Animals = new ConcurrentBag<IDisposable>();
        }
    }
}
