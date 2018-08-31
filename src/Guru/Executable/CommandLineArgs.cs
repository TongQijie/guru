using Guru.ExtensionMethod;
using Guru.Foundation;

namespace Guru.Executable
{
    public class CommandLineArgs
    {
        private readonly IgnoreCaseKeyValues<string> _Dictionary = new IgnoreCaseKeyValues<string>();

        private readonly string[] _SourceArgs;

        public CommandLineArgs(string[] sourceArgs)
        {
            _SourceArgs = sourceArgs;
        }

        public string[] Src => _SourceArgs ?? new string[0];

        public string this[int index]
        {
            get
            {
                if (_SourceArgs == null || index >= _SourceArgs.Length)
                {
                    return null;
                }

                return _SourceArgs[index];
            }
        }

        public string this[string name]
        {
            get
            {
                if (_Dictionary.ContainsKey(name))
                {
                    return _Dictionary.GetStringValue(name);
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
                        return _Dictionary.GetStringValue(name);
                    }
                }
                return null;
            }
        }

        public int Length
        {
            get
            {
                if (_SourceArgs == null)
                {
                    return 0;
                }
                else
                {
                    return _SourceArgs.Length;
                }
            }
        }

        public string[] Get(int startIndex, int count)
        {
            return _SourceArgs.Subset(startIndex, count);
        }

        public void Add(string name, string value)
        {
            _Dictionary.Add(name, value);
        }
    }
}