using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AnnotationAttribute : Attribute
    {
        public AnnotationAttribute() { }

        public AnnotationAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }

        public bool Hidden { get; set; }
    }
}