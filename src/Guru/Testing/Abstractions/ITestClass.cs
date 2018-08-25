using System;

namespace Guru.Testing.Abstractions
{
    public interface ITestClass
    {
        string Name { get; }

        Type Prototype { get; }

        ITestMethod[] GetAllMethods();

        ITestMethod GetTestMethod(string name);
    }
}
