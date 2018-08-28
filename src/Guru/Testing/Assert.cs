using Guru.DependencyInjection;
using Guru.Testing.Abstractions;
using Guru.ExtensionMethod;

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

        public static string MustHasValue(string stringValue)
        {
            if (CheckIfEnableTestMode())
            {
                if (!stringValue.HasValue())
                {
                    throw new AssertFailureException();
                }
            }
            return stringValue;
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
