namespace Guru.Testing.Abstractions
{
    public interface ITestMethod
    {
        string Name { get; }

        ITestInput[] TestInputs { get; }

        object Invoke(object instance, params object[] parameters);
    }
}