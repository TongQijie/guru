using System;

namespace Guru.AspNetCore.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Guru.Executable.AppInstance.Default.RunAsync(args);
        }
    }
}
