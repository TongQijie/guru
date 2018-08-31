using System.Collections.Generic;
using System.Linq;
using Guru.ExtensionMethod;

namespace Guru.Foundation
{
    public class IgnoreCaseKeyValues<T>
    {
        public IgnoreCaseKeyValues()
        {
            KeyValues = new List<ReadOnlyKeyValue<string, T>>();
        }

        public List<ReadOnlyKeyValue<string, T>> KeyValues { get; private set; }

        public void Add(string key, T value)
        {
            KeyValues.Add(new ReadOnlyKeyValue<string, T>(key, value));
        }

        public void AddRange(IgnoreCaseKeyValues<T> kvs)
        {
            KeyValues.AddRange(kvs.KeyValues);
        }

        public void AddRange(IEnumerable<ReadOnlyKeyValue<string, T>> kvs)
        {
            KeyValues.AddRange(kvs);
        }

        public T GetValue(string key)
        {
            var kv = KeyValues.FirstOrDefault(x => x.Key.EqualsIgnoreCase(key));
            if (kv != null)
            {
                return kv.Value;
            }
            return default(T);
        }

        public T[] GetValues(string key)
        {
            return KeyValues.Where(x => x.Key.EqualsIgnoreCase(key)).Select(x => x.Value).ToArray();
        }

        public string GetStringValue(string key)
        {
            return string.Join(";", KeyValues.Where(x => x.Key.EqualsIgnoreCase(key)).Select(x => x.Value));
        }

        public bool ContainsKey(string key)
        {
            return KeyValues.Exists(x => x.Key.EqualsIgnoreCase(key));
        }
    }
}