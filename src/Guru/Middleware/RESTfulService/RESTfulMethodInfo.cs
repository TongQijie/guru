using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using Guru.ExtensionMethod;

namespace Guru.Middleware.RESTfulService
{
    public class RESTfulMethodInfo
    {
        private readonly MethodInfo _Info;

        private readonly string _Name;
        
        private readonly bool _Default;
        
        private readonly HttpVerb _HttpVerb;
        
        private readonly ContentType _RequestContentType;
        
        private readonly ContentType _ResponseContentType;

        private readonly bool _IsAsyncMethod;

        private readonly Type[] _ReturnTypeGenericParameters;
        
        private RESTfulParameterInfo[] _ParameterInfos;

        public RESTfulMethodInfo(MethodInfo info, string name, bool @default, HttpVerb httpVerb, ContentType requestContentType, ContentType responseContentType)
        {
            _Info = info;
            _Name = name;
            _Default = @default;
            _HttpVerb = httpVerb;
            _RequestContentType = requestContentType;
            _ResponseContentType = responseContentType;

            _IsAsyncMethod = info.IsDefined(typeof(AsyncStateMachineAttribute));
            if (_IsAsyncMethod)
            {
                // async method, get return type: Task or Task<T>
                if (!info.ReturnType.GetTypeInfo().IsGenericType)
                {
                    _ReturnTypeGenericParameters = new Type[0];
                }
                else
                {
                    _ReturnTypeGenericParameters = info.ReturnType.GetGenericArguments();
                }
            }
        }

        public string Name => _Name;

        public bool Default => _Default;

        public HttpVerb HttpVerb => _HttpVerb;

        public ContentType RequestContentType => _RequestContentType;

        public ContentType ResponseContentType => _ResponseContentType;

        public RESTfulParameterInfo[] ParameterInfos => _ParameterInfos ?? new RESTfulParameterInfo[0];

        public void AddParameter(RESTfulParameterInfo info)
        {
            _ParameterInfos = _ParameterInfos.Append(info);
        }

        public object Invoke(object instance, params object[] parameters)
        {
            if (!_IsAsyncMethod)
            {
                return _Info.Invoke(instance, parameters);
            }
            else
            {
                return _HandleAsyncMethod.MakeGenericMethod(_ReturnTypeGenericParameters).Invoke(this, new object[] { _Info.Invoke(instance, parameters) });
            }
        }

        static RESTfulMethodInfo()
        {
            _HandleAsyncMethod = typeof(RESTfulMethodInfo).GetMethod("HandleAsync", BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static readonly MethodInfo _HandleAsyncMethod;

        private static T HandleAsync<T>(Task task)
        {
            return ((Task<T>)task).GetAwaiter().GetResult();
        }
    }
}