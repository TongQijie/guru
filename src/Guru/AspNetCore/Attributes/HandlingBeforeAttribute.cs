using System;

namespace Guru.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HandlingBeforeAttribute : Attribute
    {
        public abstract HandlingResult Handle(string id, Type returnType, params object[] args);
    }
}