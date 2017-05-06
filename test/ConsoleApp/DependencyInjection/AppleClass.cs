using System;

using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;

namespace ConsoleApp.DependencyInjection
{
    [Injectable(typeof(IAppleInterface), Lifetime.Transient)]
    internal class AppleClass : IAppleInterface
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