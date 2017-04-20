using System;

namespace Guru.Formatter.Json
{
    public class JElement
    {
        public string Key { get; set; }

        public Type InternalType { get; set; }

        public JElement Parent { get; set; }

        public JElement[] Children { get; set; }
    }
}