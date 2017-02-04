using System.Reflection;

namespace Guru.DependencyInjection.Abstractions
{
    public interface IAssemblyLoader
    {
        Assembly[] GetAssemblies();
    }
}