namespace Guru.Testing.Abstractions
{
    public interface ITestProvider
    {
        ITestClass[] GetAllClasses();

        object Invoke(string testClassName, string testMethodName, object[] parameters);

        void Run(string testClassName, string testMethodName);
    }
}