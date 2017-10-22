using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Logging.Abstractions;
using Guru.ExtensionMethod;

namespace Guru.Logging.Implementation
{
    [Injectable(typeof(ILoggerKeeper), Lifetime.Singleton)]
    internal class DefaultLoggerKeeper : ILoggerKeeper
    {
        private ILogger[] _Loggers = null;

        private object _Locker = new object();

        public void Connect(ILogger logger)
        {
            if (!_Loggers.Exists(x => x == logger))
            {
                lock (_Locker)
                {
                    if (!_Loggers.Exists(x => x == logger))
                    {
                        _Loggers = _Loggers.Append(logger);
                    }
                }
            }
        }

        public void Disconnect(ILogger logger)
        {
            if (_Loggers.Exists(x => x == logger))
            {
                lock (_Locker)
                {
                    if (_Loggers.Exists(x => x == logger))
                    {
                        _Loggers = _Loggers.Remove(x => x == logger);
                    }
                }
            }
        }

        public void DisposeAll()
        {
            foreach (var logger in _Loggers)
            {
                logger.Dispose();
            }
        }
    }
}
