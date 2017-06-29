using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Network;
using Guru.Network.Abstractions;

namespace Crawler
{
    public class GoogleMap
    {
        private readonly IHttpClientRequest _Request;

        private readonly IFormatter _Formatter;

        public GoogleMap()
        {
            _Request = ContainerManager.Default.Resolve<IHttpClientBroker>()
                .Get(new DefaultHttpClientSettings("GoogleMap", null, null, TimeSpan.FromSeconds(10)));

            _Formatter = ContainerManager.Default.Resolve<ITextFormatter>();
        }

        public void Run()
        {
            var airportInfos = new List<AirportInfo>();
            using (var inputStream = new FileStream("./airports.txt".FullPath(), FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(inputStream, Encoding.UTF8))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var fields = line.Split('\t');

                        airportInfos.Add(new AirportInfo()
                        {
                            AirportName = fields[0].Trim(),
                            AirportCode = fields[1].Trim(),
                            Longitude = double.Parse(fields[2]),
                            Latitude = double.Parse(fields[3]),
                        });
                    }
                }
            }


            var done = new List<Tuple<string, string>>();
            using (var inputStream = new FileStream("./output.txt".FullPath(), FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(inputStream, Encoding.UTF8))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var fields = line.Split(',');

                        done.Add(new Tuple<string, string>(fields[1], fields[3]));
                    }
                }
            }

            using (var outputStream = new FileStream("./output.txt".FullPath(), FileMode.Append, FileAccess.Write))
            {
                using (var writer = new StreamWriter(outputStream, Encoding.UTF8))
                {
                    for (int i = 0; i < airportInfos.Count; i++)
                    {
                        var from = airportInfos[i];

                        for (int j = i + 1; j < airportInfos.Count; j++)
                        {
                            var to = airportInfos[j];

                            if (done.Exists(x => x.Item1 == from.AirportCode && x.Item2 == to.AirportCode) ||
                                done.Exists(x => x.Item1 == to.AirportCode && x.Item2 == from.AirportCode))
                            {
                                continue;
                            }

                            if (CalculateGeoDistance(from.Latitude, from.Longitude, to.Latitude, to.Longitude) > 500000)
                            {
                                continue;
                            }

                            Console.Write($"{from.AirportName} -> {to.AirportName} : ");

                            var jsonString = GetDriveDistance(from.Longitude, from.Latitude, to.Longitude, to.Latitude);
                            if (!string.IsNullOrEmpty(jsonString))
                            {
                                var distance = ParseDriveDistance(jsonString);
                                if (distance > 0)
                                {
                                    writer.WriteLine($"{from.AirportName},{from.AirportCode},{to.AirportName},{to.AirportCode},{distance}");
                                    writer.WriteLine($"{to.AirportName},{to.AirportCode},{from.AirportName},{from.AirportCode},{distance}");
                                    writer.Flush();

                                    Console.WriteLine($"Success");

                                    Thread.Sleep(1000);

                                    continue;
                                }
                            }

                            Console.WriteLine($"Fail");
                        }
                    }
                }
            }
        }

        public string GetDriveDistance(double fromLng, double fromLat, double toLng, double toLat)
        {
            var url = $"https://www.google.com.hk/maps/preview/directions?authuser=0&hl=zh-CN&pb=!1m5!1s{fromLat}%2C{fromLng}!3m2!3d{fromLat}!4d{fromLng}!6e2!1m1!1s{toLat}%2C{toLng}!3m12!1m3!1d4083096.7736641993!2d114.38923183837414!3d35.43317029549576!2m3!1f0!2f0!3f0!3m2!1i1920!2i471!4f13.1!6m9!2m3!5m1!6e2!20e3!10b1!16b1!20m2!1e0!2e3!8m0!15m4!1sAVc3Wf-rO4qu0gS-5pywAg!4m1!2i10317!7e81!20m28!1m6!1m2!1i0!2i0!2m2!1i458!2i471!1m6!1m2!1i1870!2i0!2m2!1i1920!2i471!1m6!1m2!1i0!2i0!2m2!1i1920!2i20!1m6!1m2!1i0!2i451!2m2!1i1920!2i471!27b1!28m0";
            //var url = $"https://www.google.com.hk/maps/preview/directions?authuser=0&hl=zh-CN&pb=!1m5!1s{fromLat}%2C{fromLng}!3m2!3d{fromLat}!4d{fromLng}!6e2!1m1!1s{toLat}%2C{toLng}!3m9!1m3!1d154387.15305384266!2d114.7587272!3d22.8807211!2m0!3m2!1i1920!2i504!4f13.1!6m9!2m3!5m1!6e2!20e3!10b1!16b1!20m2!1e0!2e3!8m0!15m4!1s2Jc3WcuMB8TmvAT2na6IDQ!4m1!2i10317!7e81!20m28!1m6!1m2!1i0!2i0!2m2!1i458!2i504!1m6!1m2!1i1870!2i0!2m2!1i1920!2i504!1m6!1m2!1i0!2i0!2m2!1i1920!2i20!1m6!1m2!1i0!2i484!2m2!1i1920!2i504!27b1!28m0";
            //var url = $"https://www.google.com/maps/preview/directions?authuser=0&hl=zh-CN&pb=!1m5!1s{fromLat}%2C{fromLng}!3m2!3d{fromLat}!4d{fromLng}!6e2!1m5!1s{toLat}%2C{toLng}!3m2!3d{toLat}!4d{toLng}!6e2!3m12!1m3!1d3581022.006436245!2d114.62731909158751!3d35.416817027742496!2m3!1f0!2f0!3f0!3m2!1i1920!2i413!4f13.1!6m9!2m3!5m1!6e2!20e3!10b1!16b1!20m2!1e0!2e3!8m0!15m4!1sbJ03Wfu5JIntvgTbwLH4Aw!4m1!2i10317!7e81!20m28!1m6!1m2!1i0!2i0!2m2!1i458!2i413!1m6!1m2!1i1870!2i0!2m2!1i1920!2i413!1m6!1m2!1i0!2i0!2m2!1i1920!2i20!1m6!1m2!1i0!2i393!2m2!1i1920!2i413!27b1!28m0";

            //request.Request.Referer = "https://www.google.com.hk/";
            //request.Request.Headers.Add("cookie", "NID=105=ks0tFji4gLNmOCPxLCVRWQyP6x6fhk8T_RRphjbNtOKCHzT5LJmMXE4U3AmDFU2F4a_Oe8QsN3pz9tZl8lk0Rg1MaRJ5N032UVG6VsJN4e-4SezIjrIOx15lw3FGKAp5YCXaCi978HOLvEpOKCj5eyrEpKqAmdVtRtU0w4_mD66tX6iQ6MXmB1KqceOZ7VMOLf2iM2N5wNU9dkP_nGDLDEkGorh44wnZ; GOOGLE_ABUSE_EXEMPTION=ID=4512987220b6c585:TM=1496814942:C=r:IP=202.60.225.156-:S=APGng0vnbBFxipCBEVrbCuc_0BdOwzv14Q");
            //request.Request.Headers.Add("x-client-data", "CIS2yQEIpbbJAQjBtskBCPqcygEIqZ3KAQ==");
            //request.Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";


            while (true)
            {
                try
                {
                    using (var response = _Request.GetAsync(url).GetAwaiter().GetResult())
                    {
                        if (response.StatusCode == 200)
                        {
                            if (!System.IO.Directory.Exists("google"))
                            {
                                System.IO.Directory.CreateDirectory("google");
                            }

                            var jsonString = response.GetBodyAsync<string>(_Formatter).GetAwaiter().GetResult();

                            jsonString = jsonString.Substring(jsonString.IndexOf('\n') + 1);

                            using (var outputStream = new FileStream($"google\\DriveDistance_{Environment.TickCount}.json", FileMode.Create, FileAccess.Write))
                            {
                                using (var writer = new StreamWriter(outputStream, Encoding.UTF8))
                                {
                                    writer.Write("{\"request\":\"");

                                    writer.Write(url);

                                    writer.Write("\",\"response\":");

                                    writer.Write(jsonString);

                                    writer.Write("}");
                                }
                            }

                            return jsonString;
                        }
                        else
                        {
                            Console.Write(response.StatusCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Thread.Sleep(5000);
            }
        }

        public double ParseDriveDistance(string jsonString)
        {
            var match = Regex.Match(jsonString, "\\[\\d+,\"\\d+,{0,1}\\d+\\.{0,1}\\d+公里\",0\\]");
            if (match.Success)
            {
                return double.Parse(match.Value.Substring(1, match.Value.IndexOf(',') - 1));
            }

            return 0;
        }

        static double EARTH_RADIUS = 6371.0;//km 地球半径 平均值，千米

        /// <summary>
        /// 给定的经度1，纬度1；经度2，纬度2. 计算2个经纬度之间的距离。
        /// </summary>
        /// <param name="lat1">经度1</param>
        /// <param name="lng1">纬度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lng2">纬度2</param>
        /// <returns>距离（米）</returns>
        public double CalculateGeoDistance(double lat1, double lng1, double lat2, double lng2)
        {
            //用haversine公式计算球面两点间的距离。
            //经纬度转换成弧度
            lat1 = ConvertDegreesToRadians(lat1);
            lng1 = ConvertDegreesToRadians(lng1);
            lat2 = ConvertDegreesToRadians(lat2);
            lng2 = ConvertDegreesToRadians(lng2);

            //差值
            var vLon = Math.Abs(lng1 - lng2);
            var vLat = Math.Abs(lat1 - lat2);

            //h is the great circle distance in radians, great circle就是一个球体上的切面，它的圆心即是球心的一个周长最大的圆。
            var h = HaverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * HaverSin(vLon);

            var distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));

            return distance * 1000;
        }

        private double HaverSin(double theta)
        {
            var v = Math.Sin(theta / 2);
            return v * v;
        }

        /// <summary>
        /// 将角度换算为弧度。
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns>弧度</returns>
        private double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private double ConvertRadiansToDegrees(double radian)
        {
            return radian * 180.0 / Math.PI;
        }
    }
}