using System;

namespace Guru.AspNetCore.Implementation.Api.Metadata
{
    public class ApiParameterMetadata
    {
        public string ParameterName { get; set; }

        public Type ParameterType { get; set; }

        public object SampleValue { get; set; }
    }
}