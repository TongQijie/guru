using Guru.DependencyInjection;
using Guru.Testing.Abstractions;

namespace Guru.Testing
{
    public static class Assert
    {
        public static void IsTrue(bool result)
        {
            if (CheckIfEnableTestMode())
            {
                if (!result)
                {
                    throw new AssertFailureException();
                }
            }
        }

        public static void IsFalse(bool result)
        {
            if (CheckIfEnableTestMode())
            {
                if (result)
                {
                    throw new AssertFailureException();
                }
            }
        }

        private static ITestManager TestManager;

        private static bool CheckIfEnableTestMode()
        {
            if (TestManager == null)
            {
                TestManager = DependencyContainer.Resolve<ITestManager>();
            }

            return TestManager.TestModeEnabled;
        }
    }
}
