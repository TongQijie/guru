using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Logging
{
    [Injectable(typeof(IFileLogger), Lifetime.Singleton)]
    internal class FileLogger : IFileLogger
    {
        public FileLogger()
        {
            Folder = "./log".FullPath();
        }

        public string Folder { get; private set; }

        public void LogEvent(string category, Severity severity, params object[] parameters)
        {
            _Items.Enqueue(new Item(category, severity, parameters));

            if (!_IsAlive)
            {
                StartThread();
            }
        }

        private ConcurrentQueue<Item> _Items = new ConcurrentQueue<Item>();

        private bool _IsAlive = false;

        private object _SyncLocker = new object();

        private void StartThread()
        {
            if (!_IsAlive)
            {
                lock (_SyncLocker)
                {
                    if (!_IsAlive)
                    {
                        new Thread(() =>
                        {
                            while (true)
                            {
                                while (_Items.Count > 0)
                                {
                                    Item item;
                                    if (_Items.TryDequeue(out item))
                                    {
                                        Flush(item);
                                    }
                                }

                                Thread.Sleep(3000);
                            }
                        })
                        {
                            IsBackground = true,
                            Name = "FileLogger",
                        }.Start();

                        _IsAlive = true;
                    }
                }
            }
        }

        private void Flush(Item item)
        {
            try
            {
                if (!Directory.Exists(Folder))
                {
                    Directory.CreateDirectory(Folder);
                }

                var fileName = string.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
                using (var outputStream = new FileStream(Path.Combine(Folder, fileName), FileMode.Append, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(outputStream, Encoding.UTF8))
                    {
                        sw.WriteLine(item.ToString());
                    }
                }


            }
            catch (Exception) { }
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
                stringBuilder.AppendFormat("{0}|{1,-12}|{2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), Severity, Category);
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