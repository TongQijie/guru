using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Testing.Abstractions;

namespace Guru.Testing.Implementation
{
    [Injectable(typeof(ITestManager), Lifetime.Singleton)]
    internal class DefaultTestManager : ITestManager
    {
        public bool TestModeEnabled { get; private set; }

        public void EnableTestMode()
        {
            if (TestModeEnabled)
            {
                return;
            }

            TestModeEnabled = true;
        }

        public void DisableTestMode()
        {
            if (!TestModeEnabled)
            {
                return;
            }

            TestModeEnabled = false;
        }
    }
}