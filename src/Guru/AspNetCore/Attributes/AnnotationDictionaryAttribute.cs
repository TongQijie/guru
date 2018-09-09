using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AnnotationDictionaryAttribute : AnnotationAttribute
    {
        public AnnotationDictionaryAttribute(string description) : base(description) { }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
