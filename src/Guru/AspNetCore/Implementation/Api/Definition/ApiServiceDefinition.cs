using System;

namespace Guru.AspNetCore.Implementation.Api.Definition
{
    internal class ApiServiceDefinition
    {
        public ApiServiceDefinition(Type prototype, string serviceName)
        {
            ServiceName = serviceName;
            Prototype = prototype;
        }

        public string ServiceName { get; set; }

        public ApiMethodDefinition[] MethodInfos { get; set; }

        public Type Prototype { get; set; }
    }
}