using System;
using System.Reflection;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.RESTfulService
{
    [DI(typeof(IRESTfulServiceFactory), Lifetime = Lifetime.Singleton)]
    internal class RESTfulServiceFactory : IRESTfulServiceFactory
    {
        private static readonly ConcurrentDictionary<string, RESTfulServiceInfo> _ServiceInfos = new ConcurrentDictionary<string, RESTfulServiceInfo>();

        private static readonly ConcurrentDictionary<string, ServiceContext> _ServiceContexts = new ConcurrentDictionary<string, ServiceContext>();
        
        private IContainer _Container;
        
        public void Init(IContainer container, IAssemblyLoader loader)
        {
            _Container = container;

            var assemblies = loader.GetAssemblies();
            
            if (!assemblies.HasLength())
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Subset(x => x.GetTypeInfo().IsClass && 
                    x.GetTypeInfo().IsDefined(typeof(ServiceAttribute), false)))
                {
                    RegisterService(type);
                }
            }
        }
        
        public void RegisterService(Type serviceType)
        {
            var serviceAttribute = serviceType.GetTypeInfo().GetCustomAttribute<ServiceAttribute>();
            if (serviceAttribute == null)
            {
                throw new Exception($"service type '{serviceType.FullName}' does not mark attribute 'RESTfulServiceAttribute'.");
            }
            
            if (!serviceAttribute.Name.HasValue() 
                || serviceAttribute.Name.ContainsIgnoreCase("/")
                || serviceAttribute.Prefix.ContainsIgnoreCase("/"))
            {
                throw new Exception($"service name/prefix cannot be empty or may be invalid.");
            }
            
            var serviceInfo = new RESTfulServiceInfo(serviceType, serviceAttribute.Name, serviceAttribute.Prefix);
            
            foreach (var methodInfo in serviceType.GetMethods())
            {
                var info = GetMethodInfo(methodInfo);
                if (info != null)
                {
                    serviceInfo.AddMethod(info);
                }
            }
            
            _ServiceInfos.AddOrUpdate(serviceInfo.Key, serviceInfo, (n, s) => serviceInfo);
            
            _Container.RegisterSingleton(serviceType, serviceType, 0);
        }

        public ServiceContext GetService(string servicePrefix, string serviceName, string methodName, HttpVerb httpVerb)
        {
            var serviceKey = $"{(servicePrefix ?? string.Empty).ToLower()}/{(serviceName ?? string.Empty).ToLower()}";
            var cacheKey = $"{httpVerb.ToString()} {serviceKey}/{(methodName ?? string.Empty).ToLower()}";

            ServiceContext serviceContext;
            if (_ServiceContexts.TryGetValue(cacheKey, out serviceContext))
            {
                return serviceContext;
            }

            RESTfulServiceInfo serviceInfo;
            if (!_ServiceInfos.TryGetValue(serviceKey, out serviceInfo))
            {
                throw new Exception($"service key '{serviceKey}' does not exist.");
            }

            var methodInfo = serviceInfo.MethodInfos.FirstOrDefault(x =>
                (methodName.HasValue() ? x.Name.EqualsIgnoreCase(methodName) : x.Default) && 
                (x.HttpVerb == HttpVerb.Any || x.HttpVerb == httpVerb));
            if (methodInfo == null)
            {
                throw new Exception($"method '{methodName}' does not exist.");
            }

            var serviceInstance = _Container.GetImplementation(serviceInfo.ServiceType);

            serviceContext = new ServiceContext()
            {
                ServiceIntance = serviceInstance,
                ServiceInfo = serviceInfo,
                MethodInfo = methodInfo,
            };

            return _ServiceContexts.GetOrAdd(cacheKey, serviceContext);
        }
        
        private RESTfulMethodInfo GetMethodInfo(MethodInfo info)
        {
            var methodAttribute = info.GetCustomAttribute<MethodAttribute>();
            if (methodAttribute == null)
            {
                return null;
            }

            if (methodAttribute.Name.ContainsIgnoreCase("/"))
            {
                throw new Exception($"method name '{methodAttribute.Name}' is not valid.");
            }
            
            var methodInfo = new RESTfulMethodInfo(info, methodAttribute.Name, methodAttribute.Default, methodAttribute.HttpVerb, 
                methodAttribute.Request, methodAttribute.Response);
                
            foreach (var parameterInfo in info.GetParameters())
            {
                methodInfo.AddParameter(GetParameterInfo(parameterInfo));
            }
            
            return methodInfo;
        }
        
        private RESTfulParameterInfo GetParameterInfo(ParameterInfo info)
        {
            var parameterAttribute = info.GetCustomAttribute<ParameterAttribute>();
            if (parameterAttribute == null)
            {
                return new RESTfulParameterInfo(info.ParameterType, info.Name, ParameterSource.Any);
            }
            else
            {
                return new RESTfulParameterInfo(
                    info.ParameterType, 
                    parameterAttribute.Alias.HasValue() ? parameterAttribute.Alias : info.Name, 
                    parameterAttribute.Source);
            }
        }
    }
}