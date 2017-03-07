using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;

namespace Guru.Middleware.RESTfulService
{
    public class RESTfulParameterInfo
    {
        private readonly Type _ParameterType;
        
        private readonly string _ParameterName;
        
        private readonly ParameterSource _ParameterSource;
        
        public RESTfulParameterInfo(Type type, string name, ParameterSource source)
        {
            _ParameterType = type;
            _ParameterName = name;
            _ParameterSource = source;
        }

        public Type ParameterType => _ParameterType;

        public string ParameterName => _ParameterName.ToLower();

        public ParameterSource Source => _ParameterSource;
        
        public object GetParameterValue(Dictionary<string, string> queryString)
        {
            if (!queryString.ContainsKey(ParameterName))
            {
                return _ParameterType.GetDefaultValue();
            }
            else
            {
                return queryString[ParameterName].ConvertTo(_ParameterType);
            }
        }
        
        public object GetParameterValue(ContentType contentType, Stream stream)
        {
            if (contentType == ContentType.Json)
            {
                return ContainerEntry.Resolve<IJsonFormatter>().ReadObject(_ParameterType, stream);
            }
            else if (contentType == ContentType.Xml)
            {
                return ContainerEntry.Resolve<IXmlFormatter>().ReadObject(_ParameterType, stream);
            }
            else
            {
                var data = new byte[0];
                var count = 0;
                var buffer = new byte[1024];
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    data = data.Append(buffer, 0, count);
                }

                return Encoding.UTF8.GetString(data);
            }
        }

        public async Task<object> GetParameterValueAsync(IFormatter formatter, Stream stream)
        {
            return await formatter.ReadObjectAsync(_ParameterType, stream);
        }
    }
}