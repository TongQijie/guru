using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Network;
using Guru.Network.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassLib.Weather
{
    public class WeatherHttpClient
    {
        private static WeatherHttpClient _Instance = null;

        public static WeatherHttpClient Instance { get { return _Instance ?? (_Instance = new WeatherHttpClient()); } }

        private WeatherHttpClient()
        {
            _Request = ContainerManager.Default.Resolve<IHttpClientBroker>().Get(new DefaultHttpClientSettings("WeatherHttpClient"));
            _Formatter = ContainerManager.Default.Resolve<IJsonFormatter>();
        }

        private readonly IHttpClientRequest _Request;

        private readonly IFormatter _Formatter;

        public enum InterfaceEnum
        {
            Moment = 1,

            Hourly = 2,

            Daily = 3,
        }

        private Dictionary<InterfaceEnum, string> UrlMappings = new Dictionary<InterfaceEnum, string>()
        {
            { InterfaceEnum.Moment, "http://dsx.weather.com/wxd/v2/MORecord/en_US/{0}:1:{1}?api=7bb1c920-7027-4289-9c96-ae5e263980bc" },
            { InterfaceEnum.Hourly, "http://dsx.weather.com/wxd/v2/DHRecord/en_US/{0}:1:{1}?api=7bb1c920-7027-4289-9c96-ae5e263980bc" },
            { InterfaceEnum.Daily, "http://dsx.weather.com/wxd/v2/DFRecord/en_US/{0}:1:{1}?api=7bb1c920-7027-4289-9c96-ae5e263980bc" },
        };

        public async Task<T> Request<T>(InterfaceEnum interfaceName, string regionCode)
        {
            if (!regionCode.HasValue() || regionCode.Length < 2)
            {
                // TODO: log
                return default(T);
            }

            var url = string.Format(UrlMappings[interfaceName], regionCode, regionCode.Substring(0, 2));

            try
            {
                using (var response = await _Request.GetAsync(url))
                {
                    if (response.StatusCode == 200)
                    {
                        return await _Formatter.ReadObjectAsync<T>(await response.GetStreamAsync());
                    }
                    else
                    {
                        // TODO: log
                    }
                }
            }
            catch(Exception)
            {
                // TODO: log
            }

            return default(T);
        }
    }
}
