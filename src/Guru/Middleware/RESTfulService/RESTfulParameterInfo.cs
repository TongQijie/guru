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
        private readonly Type _Type;
        
        private readonly string _Name;
        
        private readonly ParameterSource _Source;
        
        public RESTfulParameterInfo(Type type, string name, ParameterSource source)
        {
            _Type = type;
            _Name = name;
            _Source = source;
        }

        public Type ParameterType => _Type;

        public string Name => _Name.ToLower();

        public ParameterSource Source => _Source;
        
        public object GetParameterValue(Dictionary<string, string> queryString)
        {
            if (!queryString.ContainsKey(Name))
            {
                return _Type.GetDefaultValue();
            }
            else
            {
                return queryString[Name].ConvertTo(_Type);
            }
        }
        
        public object GetParameterValue(ContentType contentType, Stream stream)
        {
            if (contentType == ContentType.Json)
            {
                return ContainerEntry.Resolve<IJsonFormatter>().ReadObject(_Type, stream);
            }
            else if (contentType == ContentType.Xml)
            {
                return ContainerEntry.Resolve<IXmlFormatter>().ReadObject(_Type, stream);
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
            if (formatter != null)
            {
                return await formatter.ReadObjectAsync(_Type, stream);
            }
            else
            {
                var data = new byte[0];
                var count = 0;
                var buffer = new byte[1024];
                while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    data = data.Append(buffer, 0, count);
                }

                return Encoding.UTF8.GetString(data);
            }
        }
    }
}