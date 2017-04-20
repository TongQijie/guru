using System;
using System.Collections.Concurrent;

namespace Guru.Formatter.Json
{
    public class JMemoryCache
    {
        private static ConcurrentDictionary<Type, JElement> _Instance = null;

        private static ConcurrentDictionary<Type, JElement> Instance
        {
            get { return _Instance ?? (_Instance = new ConcurrentDictionary<Type, JElement>()); }
        }

        public void Set(Type key, JElement value)
        {
            Instance.AddOrUpdate(key, value, (a, b) => b);
        }

        public JElement Get(Type key)
        {
            return Instance.GetOrAdd(key, JParser.Convert(key, null, null));
        }
    }
}