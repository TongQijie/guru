using System;

namespace Guru.Executable.Abstractions
{
    public interface IZooKeeper
    {
        void Add(IDisposable disposable);

        void RemoveAll();
    }
}