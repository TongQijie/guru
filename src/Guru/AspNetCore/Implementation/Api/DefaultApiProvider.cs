using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Attributes;
using Guru.AspNetCore.Delegates;
using Guru.AspNetCore.Implementation.Api.Definition;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Foundation;
using Guru.Logging;
using Guru.Logging.Abstractions;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiProvider), Lifetime.Singleton)]
    internal class DefaultApiProvider : IApiProvider
    {
        private readonly IApiFormatterProvider _ApiFormatters;

        private readonly IFileLogger _Logger;

        private readonly IgnoreCaseKeyValues<ApiServiceDefinition> _ApiServiceInfos = new IgnoreCaseKeyValues<ApiServiceDefinition>();

        public DefaultApiProvider(IApiFormatterProvider apiFormatters, IFileLogger logger)
        {
            _ApiFormatters = apiFormatters;
            _Logger = logger;
            Init();
        }

        public async Task<IApiContext> GetApi(CallingContext context)
        {
            if (!context.RouteData.HasLength())
            {
                return null;
            }

            var key = string.Join("/", context.RouteData.Subset(0, 2).Select(x => x.ToLower()));
            if (!_ApiContextCaches.ContainsKey(key))
            {
                if (!_ApiServiceInfos.ContainsKey(context.RouteData[0]))
                {
                    return null;
                }

                var apiServiceInfo = _ApiServiceInfos.GetValue(context.RouteData[0]);

                var apiMethodInfo = apiServiceInfo.MethodInfos.FirstOrDefault(x => 
                    (context.RouteData.Length == 1 && x.DefaultMethod) ||
                    (context.RouteData.Length >= 2 && x.MethodName.EqualsIgnoreCase(context.RouteData[1])));
                if (apiMethodInfo == null)
                {
                    return null;
                }

                var instance = DependencyContainer.Resolve(apiServiceInfo.Prototype);

                _ApiContextCaches.TryAdd(key, new ApiContextCache()
                {
                    Key = key,
                    MethodInfo = apiMethodInfo,
                    ApiExecute = args => apiMethodInfo.Invoke(instance, args),
                });
            }

            ApiContextCache cache = null;
            _ApiContextCaches.TryGetValue(key, out cache);

            if (cache == null || cache.MethodInfo == null)
            {
                return null;
            }
            
            var parameterValues = new object[cache.MethodInfo.Parameters?.Length ?? 0];
            try
            {
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    var apiParameterInfo = cache.MethodInfo.Parameters[i];

                    if (context.RouteData.Length > 2 && i < (context.RouteData.Length - 2))
                    {
                        parameterValues[i] = context.RouteData[2 + i].ConvertTo(apiParameterInfo.Prototype.ParameterType);
                    }
                    else if (context.InputParameters.ContainsKey(apiParameterInfo.ParameterName))
                    {
                        parameterValues[i] = context.InputParameters.GetValue(apiParameterInfo.ParameterName).Value.ConvertTo(apiParameterInfo.Prototype.ParameterType);
                    }
                    else
                    {
                        if (context.InputStream != null &&
                            context.InputStream.CanRead &&
                            apiParameterInfo.Prototype.ParameterType != typeof(string) &&
                            apiParameterInfo.Prototype.ParameterType.GetTypeInfo().IsClass)
                        {
                            var apiFormatter = _ApiFormatters.Get(context);
                            parameterValues[i] = await apiFormatter.Read(apiParameterInfo.Prototype.ParameterType, context.InputStream);
                        }
                        else
                        {
                            parameterValues[i] = cache.MethodInfo.Parameters[i].Prototype.ParameterType.GetDefaultValue();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.LogEvent(nameof(DefaultApiProvider), Severity.Error, "error occurs when setting parameter values.", e);
                return null;
            }

            return new DefaultApiContext()
            {
                Parameters = parameterValues,
                ApiExecute = cache.ApiExecute,
            };
        }

        #region Initialization

        private void Init()
        {
            var assemblies = AssemblyLoader.Instance.GetAssemblies();

            if (!assemblies.HasLength())
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass &&
                    x.GetTypeInfo().IsDefined(typeof(ApiAttribute), false)))
                {
                    RegisterService(type);
                }
            }
        }

        private void RegisterService(Type serviceType)
        {
            var serviceAttribute = serviceType.GetTypeInfo().GetCustomAttribute<ApiAttribute>();
            if (serviceAttribute == null)
            {
                throw new Exception($"service type '{serviceType.FullName}' does not mark attribute 'ApiServiceAttribute'.");
            }

            var apiServiceInfo = new ApiServiceDefinition(serviceType, serviceAttribute.ServiceName.Alternate(serviceType.Name));

            foreach (var methodInfo in serviceType.GetMethods())
            {
                var methodAttribute = methodInfo.GetCustomAttribute<ApiMethodAttribute>();
                if (methodAttribute == null)
                {
                    continue;
                }

                var apiMethodInfo = new ApiMethodDefinition(methodInfo, methodAttribute.MethodName.Alternate(methodInfo.Name), methodAttribute.DefaultMethod);

                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    var parameterAttribute = parameterInfo.GetCustomAttribute<ApiParameterAttribute>();

                    var apiParameterInfo = new ApiParameterDefinition(parameterInfo, (parameterAttribute?.ParameterName).Alternate(parameterInfo.Name));

                    apiMethodInfo.Parameters = apiMethodInfo.Parameters.Append(apiParameterInfo);
                }

                apiServiceInfo.MethodInfos = apiServiceInfo.MethodInfos.Append(apiMethodInfo);
            }

            DependencyContainer.RegisterSingleton(serviceType, serviceType);

            if (!_ApiServiceInfos.ContainsKey(apiServiceInfo.ServiceName))
            {
                _ApiServiceInfos.Add(apiServiceInfo.ServiceName, apiServiceInfo);
            }
        }

        #endregion

        #region ApiContext Cache

        private readonly ConcurrentDictionary<string, ApiContextCache> _ApiContextCaches = new ConcurrentDictionary<string, ApiContextCache>();

        class ApiContextCache
        {
            public string Key { get; set; }

            public ApiMethodDefinition MethodInfo { get; set; }

            public ApiExecuteDelegate ApiExecute { get; set; }
        }

        #endregion
    }
}