using Guru.Cache.Abstractions;

namespace Guru.Cache.Delegates
{
    public delegate T SetDelegate<T>(ICacheProvider provider);
}