using System;

using Guru.ExtensionMethod;

namespace Guru.Middleware.RESTfulService
{
    public class RESTfulServiceInfo
    {
        private readonly string _Name;

        private readonly string _Prefix;

        private readonly Type _ServiceType;
        
        private RESTfulMethodInfo[] _MethodInfos;
        
        public RESTfulServiceInfo(Type serviceType, string name, string prefix)
        {
            _ServiceType = serviceType;
            // service name MUST be lowercase
            _Name = name.ToLower();
            _Prefix = prefix?.ToLower();
        }

        public string Name => _Name;

        public string Prefex => _Prefix;

        public string Key => $"{_Prefix ?? string.Empty}/{_Name}"; 

        public Type ServiceType => _ServiceType;
        
        public RESTfulMethodInfo[] MethodInfos { get { return _MethodInfos ?? new RESTfulMethodInfo[0]; } }
        
        public void AddMethod(RESTfulMethodInfo info)
        {
            _MethodInfos = _MethodInfos.Append(info);
        }
    }
}