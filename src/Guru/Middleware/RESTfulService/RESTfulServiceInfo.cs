using System;

using Guru.ExtensionMethod;

namespace Guru.Middleware.RESTfulService
{
    public class RESTfulServiceInfo
    {
        private readonly string _Name;

        private readonly Type _ServiceType;
        
        private RESTfulMethodInfo[] _MethodInfos;
        
        public RESTfulServiceInfo(Type serviceType, string name)
        {
            _ServiceType = serviceType;
            _Name = name;
        }

        public string Name => _Name;

        public Type ServiceType => _ServiceType;
        
        public RESTfulMethodInfo[] MethodInfos { get { return _MethodInfos ?? new RESTfulMethodInfo[0]; } }
        
        public void AddMethod(RESTfulMethodInfo info)
        {
            _MethodInfos = _MethodInfos.Append(info);
        }
    }
}