using Guru.DependencyInjection;

namespace ConsoleApp.DependencyInjection
{
    [FileDI(typeof(IDurianInterface), "./dependencyinjection/durian_*.json", Multiply = true)]
    internal class DurianClass : IDurianInterface
    {
        public string B { get; set; }
    }
}