namespace Guru.DependencyInjection.Configuration
{
    public class DependencyConfiguration
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public Lifetime Lifetime { get; set; }

        public int Priority { get; set; }

        public DependencyPropertyConfiguration[] Properties { get; set; }
    }
}