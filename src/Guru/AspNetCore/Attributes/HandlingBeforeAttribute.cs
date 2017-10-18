using System;

namespace Guru.AspNetCore.Attributes
{
    public abstract class HandlingBeforeAttribute : Attribute
    {
        public abstract HandlingBeforeResult Handle(params object[] args);
    }
}