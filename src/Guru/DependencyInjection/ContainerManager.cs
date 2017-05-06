using Guru.DependencyInjection.Abstractions;
using Guru.DependencyInjection.Implementations;

namespace Guru.DependencyInjection
{
    public static class ContainerManager
    {
        private static IContainerInstance _Default = null;

        public static IContainerInstance Default
        {
            get
            {
                if (_Default == null)
                {
                    _Default = new ContainerInstance();

                    _Default
                        .Register(new ImplementationRegister())
                        .Register(new DynamicProxyImplementationRegister())
                        .Register(new StaticFileImplementationRegister());
                }

                return _Default;
            }
        }
    }
}