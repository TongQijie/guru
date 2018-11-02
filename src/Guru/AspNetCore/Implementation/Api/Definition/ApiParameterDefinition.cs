using System.Reflection;

namespace Guru.AspNetCore.Implementation.Api.Definition
{
    internal class ApiParameterDefinition
    {
        public ApiParameterDefinition(ParameterInfo prototype, string parameterName, string formatter)
        {
            Prototype = prototype;
            ParameterName = parameterName;
            Formatter = formatter;
        }

        public ParameterInfo Prototype { get; set; }

        public string ParameterName { get; set; }

        public string Formatter { get; set; }
    }
}