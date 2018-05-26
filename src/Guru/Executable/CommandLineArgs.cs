using Guru.ExtensionMethod;
using Guru.Foundation;

namespace Guru.Executable
{
    public class CommandLineArgs
    {
        private readonly DictionaryIgnoreCase<string> _Dictionary = new DictionaryIgnoreCase<string>();

        public string this[string name]
        {
            get
            {
                if (_Dictionary.ContainsKey(name))
                {
                    return _Dictionary.Get(name);
                }
                else
                {
                    return null;
                }
            }
        }

        public string this[string[] names]
        {
            get
            {
                if (!names.HasLength())
                {
                    return null;
                }
                foreach (var name in names)
                {
                    if (_Dictionary.ContainsKey(name))
                    {
                        return _Dictionary.Get(name);
                    }
                }
                return null;
            }
        }

        public void Add(string name, string value)
        {
            _Dictionary.AddOrUpdate(name, value);
        }
    }
}