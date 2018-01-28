using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HandlingAfterAttribute : Attribute
    {
        public abstract HandlingResult Handle(string id, Type returnType, object args);
    }
}