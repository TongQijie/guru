using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Attributes;
using Guru.AspNetCore.Delegates;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Logging;
using Guru.Logging.Abstractions;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiProvider), Lifetime.Singleton)]
    public class DefaultApiProvider : IApiProvider
    {
        private readonly IApiFormatters _ApiFormatters;

        private readonly IFileLogger _Logger;

        public DefaultApiProvider(IApiFormatters apiFormatters, IFileLogger logger)
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
                if (!_ApiServiceInfos.ContainsKey(context.RouteData[0].ToLower()))
                {
                    return null;
                }

                var apiServiceInfo = _ApiServiceInfos[context.RouteData[0].ToLower()];

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

            var apiServiceInfo = new ApiServiceInfo(serviceType, serviceAttribute.ServiceName.Alternate(serviceType.Name));

            foreach (var methodInfo in serviceType.GetMethods())
            {
                var methodAttribute = methodInfo.GetCustomAttribute<ApiMethodAttribute>();
                if (methodAttribute == null)
                {
                    continue;
                }

                var apiMethodInfo = new ApiMethodInfo(methodInfo, methodAttribute.MethodName.Alternate(methodInfo.Name), methodAttribute.DefaultMethod);

                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    var parameterAttribute = parameterInfo.GetCustomAttribute<ApiParameterAttribute>();

                    var apiParameterInfo = new ApiParameterInfo(parameterInfo, (parameterAttribute?.ParameterName).Alternate(parameterInfo.Name));

                    apiMethodInfo.Parameters = apiMethodInfo.Parameters.Append(apiParameterInfo);
                }

                apiServiceInfo.MethodInfos = apiServiceInfo.MethodInfos.Append(apiMethodInfo);
            }

            DependencyContainer.RegisterSingleton(serviceType, serviceType);

            if (!_ApiServiceInfos.ContainsKey(apiServiceInfo.ServiceName.ToLower()))
            {
                _ApiServiceInfos.Add(apiServiceInfo.ServiceName.ToLower(), apiServiceInfo);
            }
        }

        #endregion

        #region Api Information

        private readonly Dictionary<string, ApiServiceInfo> _ApiServiceInfos = new Dictionary<string, ApiServiceInfo>();

        class ApiServiceInfo
        {
            public ApiServiceInfo(Type prototype, string serviceName)
            {
                ServiceName = serviceName;
                Prototype = prototype;
            }

            public string ServiceName { get; set; }

            public ApiMethodInfo[] MethodInfos { get; set; }

            public Type Prototype { get; set; }
        }

        class ApiMethodInfo
        {
            private readonly bool _IsAsyncMethod;

            private readonly Type[] _ReturnTypeGenericParameters;

            private readonly HandlingBeforeAttribute _HandlingBefore;

            private readonly HandlingAfterAttribute _HandlingAfter;
            
            public ApiMethodInfo(MethodInfo prototype, string methodName, bool defaultMethod)
            {
                Prototype = prototype;
                MethodName = methodName;
                DefaultMethod = defaultMethod;
                _HandlingBefore = prototype.GetCustomAttribute<HandlingBeforeAttribute>();
                _HandlingAfter = prototype.GetCustomAttribute<HandlingAfterAttribute>();

                _IsAsyncMethod = prototype.IsDefined(typeof(AsyncStateMachineAttribute));
                if (_IsAsyncMethod)
                {
                    if (!prototype.ReturnType.GetTypeInfo().IsGenericType)
                    {
                        _ReturnTypeGenericParameters = new Type[1] { typeof(void) };
                    }
                    else
                    {
                        _ReturnTypeGenericParameters = prototype.ReturnType.GetGenericArguments();
                    }
                }
            }

            public object Invoke(object instance, params object[] parameters)
            {
                var id = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

                if (_HandlingBefore != null)
                {
                    var rst = _HandlingBefore.Handle(id, ReturnType, parameters);
                    if (rst == null)
                    {
                        return null;
                    }

                    if (!rst.Succeeded)
                    {
                        return rst.ResultObject;
                    }
                }

                object result;
                if (!_IsAsyncMethod)
                {
                    result = Prototype.Invoke(instance, parameters);
                }
                else
                {
                    result = _HandleAsyncMethod.MakeGenericMethod(_ReturnTypeGenericParameters).Invoke(this, new object[] { Prototype.Invoke(instance, parameters) });
                }

                if (_HandlingAfter != null)
                {
                    var rst = _HandlingAfter.Handle(id, ReturnType, result);
                    if (rst == null)
                    {
                        return null;
                    }

                    if (!rst.Succeeded)
                    {
                        return rst.ResultObject;
                    }
                }

                return result;
            }

            static ApiMethodInfo()
            {
                _HandleAsyncMethod = typeof(ApiMethodInfo).GetMethod("HandleAsync", BindingFlags.Static | BindingFlags.NonPublic);
            }

            private static readonly MethodInfo _HandleAsyncMethod;

            private static T HandleAsync<T>(Task task)
            {
                return ((Task<T>)task).GetAwaiter().GetResult();
            }

            public string MethodName { get; set; }

            public bool DefaultMethod { get; set; }

            public ApiParameterInfo[] Parameters { get; set; }

            public MethodInfo Prototype { get; set; }

            public Type ReturnType
            {
                get
                {
                    if (_IsAsyncMethod)
                    {
                        if (_ReturnTypeGenericParameters.HasLength())
                        {
                            return _ReturnTypeGenericParameters[0];
                        }
                        else
                        {
                            return typeof(void);
                        }
                    }
                    else
                    {
                        return Prototype.ReturnType;
                    }
                }
            }
        }

        class ApiParameterInfo
        {
            public ApiParameterInfo(ParameterInfo prototype, string parameterName)
            {
                Prototype = prototype;
                ParameterName = parameterName;
            }

            public ParameterInfo Prototype { get; set; }

            public string ParameterName { get; set; }
        }

        #endregion

        #region ApiContext Cache

        private readonly ConcurrentDictionary<string, ApiContextCache> _ApiContextCaches = new ConcurrentDictionary<string, ApiContextCache>();

        class ApiContextCache
        {
            public string Key { get; set; }

            public ApiMethodInfo MethodInfo { get; set; }

            public ApiExecuteDelegate ApiExecute { get; set; }
        }

        #endregion
    }
}