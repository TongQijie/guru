using Guru.Cache.Abstractions;
using System.Threading.Tasks;

namespace Guru.Cache.Delegates
{
    public delegate Task<T> SetAsyncDelegate<T>(ICacheProvider provider);
}
