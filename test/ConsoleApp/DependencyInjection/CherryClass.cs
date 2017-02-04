using Guru.DependencyInjection;

namespace ConsoleApp.DependencyInjection
{
    [FileDI(typeof(ICherryInterface), "./dependencyinjection/cherry.json")]
    public class CherryClass : ICherryInterface
    {
        public int A { get; set; }
    }
}