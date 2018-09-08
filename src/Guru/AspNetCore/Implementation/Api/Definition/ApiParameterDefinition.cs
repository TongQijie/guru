using System.Reflection;

namespace Guru.AspNetCore.Implementation.Api.Definition
{
    internal class ApiParameterDefinition
    {
        public ApiParameterDefinition(ParameterInfo prototype, string parameterName)
        {
            Prototype = prototype;
            ParameterName = parameterName;
        }

        public ParameterInfo Prototype { get; set; }

        public string ParameterName { get; set; }
    }
}