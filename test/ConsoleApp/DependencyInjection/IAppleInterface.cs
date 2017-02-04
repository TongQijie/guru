namespace ConsoleApp.DependencyInjection
{
    public interface IAppleInterface
    {
        IBananaInterface Banana { get; }

        void SayHi(string hi);
    }
}