using System;
using System.Collections.Concurrent;
using Guru.Executable.Abstractions;

namespace Guru.Executable.Implementation
{
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
