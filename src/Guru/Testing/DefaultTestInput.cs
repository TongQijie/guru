using Guru.Testing.Abstractions;

namespace Guru.Testing
{
    internal class DefaultTestInput : ITestInput
    {
        public object[] InputValues { get; private set; }

        public DefaultTestInput(params object[] values)
        {
            InputValues = values;
        }
    }
}