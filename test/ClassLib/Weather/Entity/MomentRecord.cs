namespace ClassLib.Weather.Entity
{
    using Guru.Formatter.Json;
    using Guru.Formatter.Xml;

    public class MomentRecord
    {
        [JsonIgnore]
        [XmlIgnore]
        public MOHdrObject MOHdr { get; set; }

        public MODataObject MOData { get; set; }

        public class MOHdrObject
        {
            [JsonIgnore]
            public string obsStn { get; set; }

            [JsonIgnore]
            public decimal procTm { get; set; }

            [JsonIgnore]
            public string _procTmLocal { get; set; }

            [JsonIgnore]
            public string procTmISO { get; set; }
        }

        public class MODataObject
        {
            [JsonIgnore]
            [XmlIgnore]
            public string stnNm { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string obsDayGmt { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string obsTmGmt { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string dyNght { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string locObsDay { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string locObsTm { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal tmpF { get; set; }

            /// <summary>
            /// 当前温度，格式：6
            /// </summary>
            [JsonProperty(Alias = "tmpC")]
            public decimal MomentTemperature { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal sky { get; set; }

            /// <summary>
            /// 天气条件，格式："Fair"
            /// </summary>
            [JsonProperty(Alias = "wx")]
            public string WeatherCondition { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal iconExt { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal alt { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal baroTrnd { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string baroTrndAsc { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string clds { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal dwptF { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal dwptC { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal hIF { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal hIC { get; set; }

            /// <summary>
            /// 湿度，格式：57
            /// </summary>
            [JsonProperty(Alias = "rH")]
            public decimal Humidity { get; set; }

            /// <summary>
            /// 气压，单位mb，格式：1030.0
            /// </summary>
            [JsonProperty(Alias = "pres")]
            public decimal Pressure { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal presChnge { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal visM { get; set; }

            /// <summary>
            /// 能见度，单位km，格式：9.99
            /// </summary>
            [JsonProperty(Alias = "visK")]
            public decimal Visibility { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal wCF { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal wCC { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal wDir { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string wDirAsc { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal wSpdM { get; set; }

            /// <summary>
            /// 风速，单位千米，格式：20
            /// </summary>
            [JsonProperty(Alias = "wSpdK")]
            public decimal WindSpeed { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal wSpdKn { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal tmpMx24F { get; set; }

            /// <summary>
            /// 今天最高气温，格式：6
            /// </summary>
            [JsonProperty(Alias = "tmpMx24C")]
            public decimal MaximumTemperature { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal tmpMn24F { get; set; }

            /// <summary>
            /// 今天最低气温，格式：-2
            /// </summary>
            [JsonProperty(Alias = "tmpMn24C")]
            public decimal MinimumTemperature { get; set; }
            
            [JsonIgnore]
            [XmlIgnore]
            public decimal prcp24 { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal prcp3_6hr { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal prcpHr { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal snwDep { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal snwIncr { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal snwTot6hr { get; set; }

            /// <summary>
            /// 日出时间，格式："06:46 am"
            /// </summary>
            [JsonProperty(Alias = "sunrise")]
            public string Sunrise { get; set; }

            /// <summary>
            /// 日落时间，格式："05:29 pm"
            /// </summary>
            [JsonProperty(Alias = "sunset")]
            public string Sunset { get; set; }

            /// <summary>
            /// 紫外线指数，格式：3
            /// </summary>
            [JsonProperty(Alias = "uvIdx")]
            public decimal UvIndex { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string uvDes { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal uvWrn { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal flsLkIdxF { get; set; }

            /// <summary>
            /// 体感温度，格式：2
            /// </summary>
            [JsonProperty(Alias = "flsLkIdxC")]
            public decimal FeelsLike { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string wxMan { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal _presIn { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal _snwDepCm { get; set; }

            /// <summary>
            /// 降雨量，单位cm，格式：0.0
            /// </summary>
            [JsonProperty(Alias = "_prcp24Cm")]
            public decimal Precipitation { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public decimal _prcp24Mm { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string _sunriseISOLocal { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string _sunsetISOLocal { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string obsDateTimeISO { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string sunriseISO { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string sunsetISO { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string _obsDateLocalTimeISO { get; set; }

            [JsonIgnore]
            [XmlIgnore]
            public string _wDirAsc_en { get; set; }
        }
    }
}
