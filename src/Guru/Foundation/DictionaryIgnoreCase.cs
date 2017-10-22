using System.Collections.Generic;

namespace Guru.Foundation
{
    public class DictionaryIgnoreCase<T>
    {
        private Dictionary<string, T> _Dictionary = new Dictionary<string, T>();

        public void AddOrUpdate(string key, T value)
        {
            var k = ConvertKey(key);

            if (_Dictionary.ContainsKey(k))
            {
                _Dictionary[k] = value;
            }
            else
            {
                _Dictionary.Add(k, value);
            }
        }

        public T Get(string key)
        {
            var k = ConvertKey(key);
            if (_Dictionary.ContainsKey(k))
            {
                return _Dictionary[k];
            }

            return default(T);
        }

        public T Get(string key, T defaultValue)
        {
            var k = ConvertKey(key);
            if (_Dictionary.ContainsKey(k))
            {
                return _Dictionary[k];
            }

            return defaultValue;
        }

        public bool ContainsKey(string key)
        {
            return _Dictionary.ContainsKey(ConvertKey(key));
        }

        public Dictionary<string, T> GetDictionary()
        {
            return _Dictionary;
        }

        private string ConvertKey(string key)
        {
            return key.ToLower();
        }
    }
}