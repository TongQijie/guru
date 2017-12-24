using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Logging.Implementation
{
    [Injectable(typeof(IFileLogger), Lifetime.Singleton)]
    internal class DefaultFileLogger : AbstractLogger, IFileLogger
    {
        public DefaultFileLogger(ILoggerKeeper loggerKeeper) : base(loggerKeeper)
        {
            Folder = "./DefaultLog".FullPath();
            Interval = 3000;
        }

        public string Folder { get; set; }

        public int Interval { get; set; }

        private string LoggerName => $"LogThread({Folder.Name()})";

        public override void LogEvent(string category, Severity severity, params object[] parameters)
        {
            _Items.Enqueue(new Item(category, severity, parameters));

            if (!_IsAlive)
            {
                StartThread();
            }
        }

        private ConcurrentQueue<Item> _Items = new ConcurrentQueue<Item>();

        private bool _IsAlive = false;

        private bool _IsThreadRunning = false;

        private object _SyncLocker = new object();

        private void StartThread()
        {
            if (!_IsAlive)
            {
                lock (_SyncLocker)
                {
                    if (!_IsAlive)
                    {
                        _IsAlive = true;

                        new Thread(() =>
                        {
                            _IsThreadRunning = true;

                            while (_IsAlive)
                            {
                                var items = new List<Item>();
                                while (_Items.TryDequeue(out var item))
                                {
                                    items.Add(item);
                                }

                                if (items.Count > 0)
                                {
                                    Flush(items);
                                }

                                Thread.Sleep(Interval);
                            }

                            _IsThreadRunning = false;
                        })
                        {
                            IsBackground = true,
                            Name = LoggerName,
                        }.Start();
                    }
                }
            }
        }

        private void Flush(IEnumerable<Item> items)
        {
            try
            {
                Folder.EnsureFolder();

                var fileName = string.Format("{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                using (var outputStream = new FileStream(Path.Combine(Folder, fileName), FileMode.Append, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(outputStream, Encoding.UTF8))
                    {
                        foreach (var item in items)
                        {
                            sw.WriteLine(item.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"failed to flush in file logger. {e.Message}");
            }
        }

        public override void Dispose()
        {
            _IsAlive = false;

            Console.WriteLine($"{LoggerName} is disposing...");

            while (_IsThreadRunning)
            {
                Thread.Sleep(500);
            }

            var items = new List<Item>();
            while (_Items.TryDequeue(out var item))
            {
                items.Add(item);
            }

            if (items.Count > 0)
            {
                Flush(items);
            }

            Console.WriteLine($"{LoggerName} disposed.");

            base.Dispose();
        }

        class Item
        {
            public Item(string category, Severity severity, object[] parameters)
            {
                Category = category;
                Severity = severity;
                Parameters = parameters;
                Timestamp = DateTime.Now;
            }

            public string Category { get; set; }

            public Severity Severity { get; set; }

            public object[] Parameters { get; set; }

            public DateTime Timestamp { get; private set; }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}|{1,-12}|{2}", Timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff"), Severity, Category);
                stringBuilder.AppendLine();
                if (Parameters != null)
                {
                    foreach (var parameter in Parameters.Subset(x => x != null))
                    {
                        if (parameter is Exception)
                        {
                            stringBuilder.Append(new ExceptionWrapper(parameter as Exception).ToString());
                        }
                        else
                        {
                            stringBuilder.Append(parameter.ToString());
                        }
                        stringBuilder.AppendLine();
                    }
                }

                return stringBuilder.ToString();
            }
        }
    }
}