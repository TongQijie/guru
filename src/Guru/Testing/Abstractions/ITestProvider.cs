namespace Guru.Testing.Abstractions
{
    public interface ITestProvider
    {
        ITestClass[] GetAllClasses();

        ITestMethod GetTestMethod(string testClassName, string testMethodName);
    }
}