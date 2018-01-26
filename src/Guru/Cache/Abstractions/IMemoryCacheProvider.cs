using System;

namespace Guru.Cache.Abstractions
{
    public interface IMemoryCacheProvider : ICacheProvider, IDisposable
    {
        bool Persistent { get; set; }

        int SecondsToClean { get; set; }
    }
}