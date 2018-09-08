using Guru.AspNetCore.Attributes;
using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using Guru.AspNetCore.Implementation.Api.Metadata;
using System.Text;
using Guru.Formatter.Abstractions;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection.Attributes;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiMetadataProvider), Lifetime.Singleton)]
    internal class DefaultApiMetadataProvider : IApiMetadataProvider
    {
        private ApiServiceMetadata[] _ApiServiceMetadatas;

        private ILightningFormatter _Formatter;

        public DefaultApiMetadataProvider(IJsonLightningFormatter jsonLightningFormatter)
        {
            jsonLightningFormatter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            _Formatter = jsonLightningFormatter;
            Initialize();
        }

        public string GetListHtml()
        {
            var stringBuilder = new StringBuilder();
            if (!_ApiServiceMetadatas.HasLength())
            {
                return string.Empty;
            }
            foreach (var apiServiceMetadata in _ApiServiceMetadatas)
            {
                stringBuilder.Append($"<h3>{apiServiceMetadata.ServiceName}</h3>");
                if (apiServiceMetadata.Methods.HasLength())
                {
                    foreach (var method in apiServiceMetadata.Methods)
                    {
                        stringBuilder.Append($"<h5><a href=\"_metadata/{apiServiceMetadata.ServiceName}/{method.MethodName}\">{method.MethodName}</a></h5>");
                    }
                }
            }
            return stringBuilder.ToString();
        }

        public string GetMethodHtml(string serviceName, string methodName)
        {
            var apiMethodMetadata = GetMethod(serviceName, methodName);
            if (apiMethodMetadata == null)
            {
                return "<h3>No method found.</h3>";
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"<h3>{serviceName} - {methodName}</h3>");
            if (apiMethodMetadata.InputParameters.HasLength())
            {
                stringBuilder.Append($"<h4>REQUEST</h4>");
                if (apiMethodMetadata.InputParameters.Length == 1 && 
                    apiMethodMetadata.InputParameters[0].ParameterType.IsClass &&
                    apiMethodMetadata.InputParameters[0].ParameterType != typeof(string))
                {
                    stringBuilder.Append($"<pre>{BuildSampleValue(apiMethodMetadata.InputParameters[0])}</pre>");
                }
                else
                {
                    foreach (var parameter in apiMethodMetadata.InputParameters)
                    {
                        stringBuilder.Append($"<h5>{parameter.ParameterName}</h5>");
                        stringBuilder.Append($"<pre>{BuildSampleValue(parameter)}</pre>");
                    }
                }
            }
            if (apiMethodMetadata.OutputParameter != null && apiMethodMetadata.OutputParameter.ParameterType != null)
            {
                if (apiMethodMetadata.OutputParameter.ParameterType != typeof(void))
                {
                    stringBuilder.Append($"<h4>RESPONSE</h4>");
                    stringBuilder.Append($"<pre>{BuildSampleValue(apiMethodMetadata.OutputParameter)}</pre>");
                }
            }
            return stringBuilder.ToString();
        }

        private void Initialize()
        {
            var assemblies = AssemblyLoader.Instance.GetAssemblies();

            if (!assemblies.HasLength())
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                foreach (var serviceType in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass &&
                    x.GetTypeInfo().IsDefined(typeof(ApiAttribute), false)))
                {
                    var serviceAttribute = serviceType.GetTypeInfo().GetCustomAttribute<ApiAttribute>();
                    if (serviceAttribute == null)
                    {
                        continue;
                    }

                    var apiServiceMetadata = new ApiServiceMetadata()
                    {
                        ServiceName = serviceAttribute.ServiceName.Alternate(serviceType.Name),
                    };

                    foreach (var methodInfo in serviceType.GetMethods())
                    {
                        var methodAttribute = methodInfo.GetCustomAttribute<ApiMethodAttribute>();
                        if (methodAttribute == null)
                        {
                            continue;
                        }

                        var apiMethodMetadata = new ApiMethodMetadata()
                        {
                            MethodName = methodAttribute.MethodName.Alternate(methodInfo.Name),
                        };

                        foreach (var parameterInfo in methodInfo.GetParameters())
                        {
                            var parameterAttribute = parameterInfo.GetCustomAttribute<ApiParameterAttribute>();

                            var apiParameterMetadata = new ApiParameterMetadata()
                            {
                                ParameterName = (parameterAttribute?.ParameterName).Alternate(parameterInfo.Name),
                                ParameterType = parameterInfo.ParameterType,
                            };

                            apiMethodMetadata.InputParameters = apiMethodMetadata.InputParameters.Append(apiParameterMetadata);
                        }

                        if (methodInfo.IsDefined(typeof(AsyncStateMachineAttribute)))
                        {
                            if (!methodInfo.ReturnType.GetTypeInfo().IsGenericType)
                            {
                                apiMethodMetadata.OutputParameter = new ApiParameterMetadata()
                                {
                                    ParameterType = typeof(void),
                                };
                            }
                            else
                            {
                                apiMethodMetadata.OutputParameter = new ApiParameterMetadata()
                                {
                                    ParameterType = methodInfo.ReturnType.GetGenericArguments().First(),
                                };
                            }
                        }
                        else
                        {
                            apiMethodMetadata.OutputParameter = new ApiParameterMetadata()
                            {
                                ParameterType = methodInfo.ReturnType,
                            };
                        }

                        apiServiceMetadata.Methods = apiServiceMetadata.Methods.Append(apiMethodMetadata);
                    }

                    _ApiServiceMetadatas = _ApiServiceMetadatas.Append(apiServiceMetadata);
                }
            }
        }

        private ApiMethodMetadata GetMethod(string serviceName, string methodName)
        {
            var apiServiceMetadata = _ApiServiceMetadatas.FirstOrDefault(x => x.ServiceName.EqualsIgnoreCase(serviceName));
            if (apiServiceMetadata == null)
            {
                return null;
            }

            return apiServiceMetadata.Methods.FirstOrDefault(x => x.MethodName.EqualsIgnoreCase(methodName));
        }

        private string BuildSampleValue(ApiParameterMetadata apiParameterMetadata)
        {
            if (apiParameterMetadata == null || apiParameterMetadata.ParameterType == null)
            {
                return string.Empty;
            }

            if (apiParameterMetadata.SampleValue == null)
            {
                apiParameterMetadata.SampleValue = new SampleValueBuilder().Build(apiParameterMetadata.ParameterType);
            }

            if (apiParameterMetadata.SampleValue != null)
            {
                if (apiParameterMetadata.SampleValue.GetType().IsValueType ||
                    apiParameterMetadata.SampleValue.GetType() == typeof(string))
                {
                    return apiParameterMetadata.SampleValue.ToString();
                }
                else
                {
                    return _Formatter.WriteObject(apiParameterMetadata.SampleValue);
                }
            }

            return string.Empty;
        }

        class SampleValueBuilder
        {
            public SampleValueBuilder() { }

            public SampleValueBuilder(List<Type> refs)
            {
                Refs.AddRange(refs);
            }

            private readonly List<Type> Refs = new List<Type>();

            private bool CheckIfLoopRef(Type type)
            {
                if (type.IsValueType || type == typeof(string))
                {
                    return false;
                }
                else
                {
                    return Refs.Contains(type);
                }
            }

            public object Build(Type type)
            {
                if (type.IsValueType)
                {
                    return type.GetDefaultValue();
                }
                else if (type == typeof(string))
                {
                    return string.Empty;
                }
                else if (type.IsArray)
                {
                    Refs.Add(type);
                    if (CheckIfLoopRef(type.GetElementType()))
                    {
                        return type.GetDefaultValue();
                    }
                    else
                    {
                        return type.CreateInstance(new SampleValueBuilder(Refs).Build(type.GetElementType()));
                    }
                }
                else if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type))
                {
                    Refs.Add(type);
                    var args = type.GetGenericArguments();
                    var instance = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(args)) as IDictionary;
                    if (args.Length != 2)
                    {
                        return null;
                    }
                    var key = new SampleValueBuilder(Refs).Build(args[0]);
                    var value = new SampleValueBuilder(Refs).Build(args[1]);
                    instance.Add(key, value);
                    return instance;
                }
                else if (type.IsClass)
                {
                    Refs.Add(type);
                    var instance = type.CreateInstance();
                    var properties = type.GetProperties();
                    foreach (var property in properties)
                    {
                        if (CheckIfLoopRef(property.PropertyType))
                        {
                            property.SetValue(instance, property.PropertyType.GetDefaultValue());
                        }
                        else
                        {
                            property.SetValue(instance, new SampleValueBuilder(Refs).Build(property.PropertyType));
                        }
                    }
                    return instance;
                }
                else
                {
                    return type.GetDefaultValue();
                }
            }
        }
    }
}