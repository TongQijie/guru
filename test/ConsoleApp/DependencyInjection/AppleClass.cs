using System;

using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace ConsoleApp.DependencyInjection
{
    [DI(typeof(IAppleInterface), Lifetime = Lifetime.Transient)]
    public class AppleClass : IAppleInterface
    {
        private readonly IBananaInterface _Banana;

        public AppleClass(IBananaInterface banana)
        {
            _Banana = banana;
        }

        public IBananaInterface Banana { get { return _Banana; } }

        public void SayHi(string hi)
        {
            Console.WriteLine("apple: '{0}'.", hi);
        }
    }
}