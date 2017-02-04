using System;
using System.Reflection;

namespace Guru.DynamicProxy
{
    public interface IInvocation
    {
        void Process();

        MethodInfo MethodInfo { get; }

        object[] ParameterValues { get; }

        object ReturnValue { get; set; }

        Type TargetType { get; }

        Type InterceptorType { get; }
    }
}
