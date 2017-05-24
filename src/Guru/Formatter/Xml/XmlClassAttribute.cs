using System;

namespace Guru.Formatter.Xml
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlClassAttribute : Attribute
    {
        public string ArrayElementName { get; set; }

        public Type ArrayElementType { get; set; }

        public string RootName { get; set; }
    }
}