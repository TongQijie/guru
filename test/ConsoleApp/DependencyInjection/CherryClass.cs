using Guru.DependencyInjection.Attributes;

namespace ConsoleApp.DependencyInjection
{
    [StaticFile(typeof(ICherryInterface), "./dependencyinjection/cherry.json")]
    public class CherryClass : ICherryInterface
    {
        public int A { get; set; }
    }
}