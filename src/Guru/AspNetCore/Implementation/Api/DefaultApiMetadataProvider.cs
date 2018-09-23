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
using Guru.Formatter.Json;

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
            stringBuilder.Append("<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body>");
            stringBuilder.Append($"<h3>{serviceName}/{methodName}</h3>");
            if (apiMethodMetadata.InputParameters.HasLength())
            {
                stringBuilder.Append($"<h4>REQUEST</h4>");
                if (apiMethodMetadata.InputParameters.Length == 1 && 
                    apiMethodMetadata.InputParameters[0].ParameterType.IsClass &&
                    apiMethodMetadata.InputParameters[0].ParameterType != typeof(string))
                {
                    stringBuilder.Append($"<pre>{BuildSampleValue(apiMethodMetadata.InputParameters[0])}</pre>");
                    BuildTypeAnnotation(stringBuilder, apiMethodMetadata.InputParameters[0].ParameterType);
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
                    BuildTypeAnnotation(stringBuilder, apiMethodMetadata.OutputParameter.ParameterType);
                }
            }
            stringBuilder.Append("</body></html>");
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

        private void BuildTypeAnnotation(StringBuilder stringBuilder, Type type)
        {
            var annotationFields = new AnnotationParser().Parse(type);
            if (annotationFields.HasLength())
            {
                stringBuilder.Append("<table border=\"1\" cellspacing=\"0\" cellpadding=\"5\"><thead><tr>");
                var fieldCount = annotationFields.Max(x => x.TopLevel);
                for (int i = 0; i < fieldCount; i++)
                {
                    stringBuilder.Append("<td>Field</td>");
                }
                stringBuilder.Append("<td>DataType</td>");
                stringBuilder.Append("<td>Annotation</td>");
                stringBuilder.Append("</tr></thead><tbody>");
                foreach (var annotationField in annotationFields)
                {
                    BuildAnnotationField(stringBuilder, annotationField, 0, fieldCount);
                }
                stringBuilder.Append("</tbody></table>");
            }
        }

        private void BuildAnnotationField(StringBuilder stringBuilder, AnnotationField annotationField, int startField, int fieldCount)
        {
            stringBuilder.Append("<tr>");
            for (int i = 0; i < startField; i++)
            {
                stringBuilder.Append("<td></td>");
            }
            stringBuilder.Append($"<td>{annotationField.Name}</td>");
            for (int i = startField + 1; i < fieldCount; i++)
            {
                stringBuilder.Append("<td></td>");
            }
            stringBuilder.Append($"<td>{annotationField.DataType}</td><td>{annotationField.Description}</td>");
            stringBuilder.Append("</tr>");
            if (annotationField.InnerAnnotationFields.HasLength())
            {
                foreach (var innerAnnotationField in annotationField.InnerAnnotationFields)
                {
                    BuildAnnotationField(stringBuilder, innerAnnotationField, startField + 1, fieldCount);
                }
            }
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
                    return "String";
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
                else if (typeof(IList).GetTypeInfo().IsAssignableFrom(type))
                {
                    var elementType = type.GetGenericArguments().FirstOrDefault() ?? (type.BaseType.GetGenericArguments().FirstOrDefault() ?? typeof(object));
                    var instance = Activator.CreateInstance(type) as IList;
                    instance.Add(new SampleValueBuilder(Refs).Build(elementType));
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

        class AnnotationField
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public bool IsCollection { get; set; }

            public string DataType { get; set; }

            public AnnotationField[] InnerAnnotationFields { get; set; }

            public int TopLevel
            {
                get
                {
                    if (InnerAnnotationFields.HasLength())
                    {
                        return InnerAnnotationFields.Max(x => x.TopLevel) + 1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }

        class AnnotationParser
        {
            public AnnotationParser() { }

            public AnnotationParser(List<Type> refs)
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

            private bool CheckIfSupportParsing(Type type)
            {
                if (type.IsClass && type != typeof(string))
                {
                    if (type.IsArray ||
                        typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type) ||
                        typeof(ICollection).GetTypeInfo().IsAssignableFrom(type))
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public AnnotationField[] Parse(Type type)
            {
                if (!CheckIfSupportParsing(type))
                {
                    return null;
                }

                if (CheckIfLoopRef(type))
                {
                    return null;
                }

                Refs.Add(type);

                var annotationFields = new AnnotationField[0];
                foreach (var property in type.GetProperties())
                {
                    var annotationAttr = property.GetCustomAttribute<AnnotationAttribute>();
                    if (annotationAttr != null && annotationAttr.Hidden)
                    {
                        continue;
                    }

                    var jsonPropertyAttr = property.GetCustomAttribute<JsonPropertyAttribute>();

                    var annotationField = new AnnotationField()
                    {
                        Name = jsonPropertyAttr?.Alias.Alternate(property.Name),
                        Description = annotationAttr?.Description ?? string.Empty,
                        IsCollection = property.PropertyType.IsArray || typeof(ICollection).GetTypeInfo().IsAssignableFrom(property.PropertyType),
                    };

                    if (property.PropertyType.IsArray)
                    {
                        var elementType = property.PropertyType.GetElementType();
                        annotationField.InnerAnnotationFields = new AnnotationParser(Refs).Parse(elementType);
                    }
                    else if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(property.PropertyType))
                    {
                        var arguments = property.PropertyType.GetGenericArguments();
                        if (arguments.LengthEqual(2))
                        {
                            

                            var keyType = arguments[0];
                            annotationField.InnerAnnotationFields = annotationField.InnerAnnotationFields.Append(new AnnotationField()
                            {
                                Name = "key",
                                DataType = GetDataType(keyType),
                                Description = (annotationAttr as AnnotationDictionaryAttribute)?.Key ?? string.Empty,
                                InnerAnnotationFields = new AnnotationParser(Refs).Parse(keyType),
                            });

                            var valueType = arguments[1];
                            annotationField.InnerAnnotationFields = annotationField.InnerAnnotationFields.Append(new AnnotationField()
                            {
                                Name = "value",
                                DataType = GetDataType(valueType),
                                Description = (annotationAttr as AnnotationDictionaryAttribute)?.Value ?? string.Empty,
                                InnerAnnotationFields = new AnnotationParser(Refs).Parse(valueType),
                            });
                        }
                    }
                    else if (typeof(ICollection).GetTypeInfo().IsAssignableFrom(property.PropertyType))
                    {
                        var elementType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                        if (elementType != null)
                        {
                            annotationField.InnerAnnotationFields = new AnnotationParser(Refs).Parse(elementType);
                        }
                    }
                    else if (property.PropertyType != typeof(string) && property.PropertyType.IsClass)
                    {
                        annotationField.InnerAnnotationFields = new AnnotationParser(Refs).Parse(property.PropertyType);
                    }
                    else
                    {
                        annotationField.DataType = property.PropertyType.Name.ToLower();
                    }

                    annotationField.DataType = GetDataType(property.PropertyType);
                    annotationFields = annotationFields.Append(annotationField);
                }

                return annotationFields;
            }

            private string GetDataType(Type type)
            {
                if (type.IsArray)
                {
                    return "list";
                }
                else if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type))
                {
                    return "dictionary";
                }
                else if (typeof(ICollection).GetTypeInfo().IsAssignableFrom(type))
                {
                    return "list";
                }
                else if (type != typeof(string) && type.IsClass)
                {
                    return "object";
                }
                else if (type.IsEnum)
                {
                    return "string";
                }
                else
                {
                    return type.Name.ToLower();
                }
            }
        }
    }
}