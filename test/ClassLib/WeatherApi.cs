using ClassLib.Weather;
using ClassLib.Weather.Entity;
using Guru.AspNetCore.Attributes;
using System.Threading.Tasks;

namespace ClassLib
{
    [ApiService("weather")]
    public class WeatherApi
    {
        [ApiMethod("moment")]
        public async Task<MomentRecord.MODataObject> GetMomentRecord(string regionCode)
        {
            return (await WeatherHttpClient.Instance.Request<MomentRecord>(WeatherHttpClient.InterfaceEnum.Moment, regionCode)).MOData;
        }

        [ApiMethod("hourly")]
        public async Task<HourlyRecords.DHDataObject[]> GetHourlyRecords(string regionCode)
        {
            return (await WeatherHttpClient.Instance.Request<HourlyRecords>(WeatherHttpClient.InterfaceEnum.Hourly, regionCode)).DHData;
        }

        [ApiMethod("daily")]
        public async Task<DailyRecords.DFDataObject[]> GetDailyRecords(string regionCode)
        {
            return (await WeatherHttpClient.Instance.Request<DailyRecords>(WeatherHttpClient.InterfaceEnum.Daily, regionCode)).DFData;
        }
    }
}
