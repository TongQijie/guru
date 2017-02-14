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
    public class RESTfulServiceFactory : IRESTfulServiceFactory
    {
        private static readonly ConcurrentDictionary<string, RESTfulServiceInfo> _ServiceInfos = new ConcurrentDictionary<string, RESTfulServiceInfo>();
        
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
            
            if (!serviceAttribute.Name.HasValue())
            {
                throw new Exception($"service name cannot be empty.");
            }
            
            var serviceInfo = new RESTfulServiceInfo(serviceType, serviceAttribute.Name);
            
            foreach (var methodInfo in serviceType.GetMethods())
            {
                var info = GetMethodInfo(methodInfo);
                if (info != null)
                {
                    serviceInfo.AddMethod(info);
                }
            }
            
            _ServiceInfos.AddOrUpdate(serviceInfo.Name, serviceInfo, (n, s) => serviceInfo);
            
            _Container.RegisterSingleton(serviceType, serviceType, 0);
        }

        public ServiceContext GetService(string serviceName, string methodName, HttpVerb httpVerb)
        {
            RESTfulServiceInfo serviceInfo;
            if (!_ServiceInfos.TryGetValue(serviceName.ToLower(), out serviceInfo))
            {
                throw new Exception($"service '{serviceName}' does not exist.");
            }

            var methodInfo = serviceInfo.MethodInfos.FirstOrDefault(x => x.Name.EqualsIgnoreCase(methodName) && (x.HttpVerb == HttpVerb.Any || x.HttpVerb == httpVerb));
            if (methodInfo == null)
            {
                throw new Exception($"method '{methodName}' does not exist.");
            }

            var serviceInstance = _Container.GetImplementation(serviceInfo.ServiceType);

            return new ServiceContext()
            {
                ServiceIntance = serviceInstance,
                ServiceInfo = serviceInfo,
                MethodInfo = methodInfo,
            };
        }
        
        private RESTfulMethodInfo GetMethodInfo(MethodInfo info)
        {
            var methodAttribute = info.GetCustomAttribute<MethodAttribute>();
            if (methodAttribute == null)
            {
                return null;
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