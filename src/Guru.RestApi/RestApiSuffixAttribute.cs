using System;
using Guru.AspNetCore.Attributes;

namespace Guru.RestApi
{
    public class RestApiSuffixAttribute : HandlingAfterAttribute
    {
        public override HandlingResult Handle(string id, Type returnType, object args)
        {
            throw new NotImplementedException();
        }
    }
}