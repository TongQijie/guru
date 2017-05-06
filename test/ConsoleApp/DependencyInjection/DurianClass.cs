using Guru.DependencyInjection.Attributes;
using System.Collections.Generic;

namespace ConsoleApp.DependencyInjection
{
    [StaticFile(typeof(IDurianInterface), "./dependencyinjection/durian_*.json", MultiFiles = true)]
    internal class DurianClass : List<string>, IDurianInterface
    {
        public string[] B { get { return this.ToArray(); } }
    }
}