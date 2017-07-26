namespace ClassLib.Weather.Entity
{
    using Guru.Formatter.Json;

    public class DailyRecords
    {
        [JsonIgnore]
        public DFHdrObject DFHdr { get; set; }

        public DFDataObject[] DFData { get; set; }

        public class DFHdrObject
        {
            [JsonIgnore]
            public string coopId { get; set; }

            [JsonIgnore]
            public string stnNm { get; set; }

            [JsonIgnore]
            public decimal procTm { get; set; }

            [JsonIgnore]
            public string _procTmLocal { get; set; }

            [JsonIgnore]
            public string procTmISO { get; set; }
        }

        public class DFDataObject
        {
            [JsonIgnore]
            public decimal dyNum { get; set; }

            /// <summary>
            /// 日期，格式："Tuesday"
            /// </summary>
            [JsonProperty(Alias = "dow")]
            public string DayOfWeek { get; set; }

            [JsonIgnore]
            public decimal fcstValGMT { get; set; }

            [JsonIgnore]
            public string fcstValDay { get; set; }

            /// <summary>
            /// 日期，格式："20160202"
            /// </summary>
            [JsonProperty(Alias = "locValDay")]
            public string Date { get; set; }

            [JsonIgnore]
            public string locValTm { get; set; }

            [JsonIgnore]
            public decimal hiTmpF { get; set; }

            [JsonIgnore]
            public decimal loTmpF { get; set; }

            /// <summary>
            /// 当日最高气温
            /// </summary>
            [JsonProperty(Alias = "hiTmpC")]
            public decimal HighTemperature { get; set; }

            /// <summary>
            /// 当日最低气温
            /// </summary>
            [JsonProperty(Alias = "loTmpC")]
            public decimal LowTemperature { get; set; }

            [JsonIgnore]
            public decimal sky12_24 { get; set; }

            [JsonIgnore]
            public decimal sky12_24X { get; set; }

            [JsonIgnore]
            public string shrtCst { get; set; }

            /// <summary>
            /// 晚上天气条件，格式："Mostly Clear"
            /// </summary>
            [JsonProperty(Alias = "snsblWx12_24")]
            public string NightWeatherCondition { get; set; }

            [JsonIgnore]
            public string tSnsblWx12_24 { get; set; }

            [JsonIgnore]
            public string wrlsWx12_24 { get; set; }
            
            /// <summary>
            /// 晚上降雨概率，格式：20
            /// </summary>
            [JsonProperty(Alias = "pOP12_24")]
            public decimal ChanceOfRainOfNight { get; set; }

            [JsonIgnore]
            public decimal rh12_24 { get; set; }

            [JsonIgnore]
            public decimal wCF { get; set; }

            [JsonIgnore]
            public decimal wCF12_24 { get; set; }

            [JsonIgnore]
            
            public decimal wCC { get; set; }

            [JsonIgnore]
            public decimal wCC12_24 { get; set; }

            [JsonIgnore]
            public decimal wSpdM12_24 { get; set; }

            [JsonIgnore]
            public decimal wSpdK12_24 { get; set; }

            [JsonIgnore]
            public decimal wSpdKn12_24 { get; set; }

            [JsonIgnore]
            public decimal wDir12_24 { get; set; }

            [JsonIgnore]
            public string wDirAsc12_24 { get; set; }

            [JsonIgnore]
            public string _wDirAsc12_24_en { get; set; }

            [JsonIgnore]
            public decimal clds12_24 { get; set; }

            [JsonIgnore]
            public string sunrise { get; set; }

            [JsonIgnore]
            public string sunset { get; set; }

            [JsonIgnore]
            public string mnRise { get; set; }

            [JsonIgnore]
            public string mnSet { get; set; }

            [JsonIgnore]
            public decimal qpf12_24 { get; set; }

            [JsonIgnore]
            public decimal qpf { get; set; }

            [JsonIgnore]
            public decimal snwQpf12_24 { get; set; }

            [JsonIgnore]
            public decimal snwQpf { get; set; }

            [JsonIgnore]
            public string precipTyp12_24 { get; set; }

            [JsonIgnore]
            public string wxMan12_24 { get; set; }

            [JsonIgnore]
            public string subPhrs12_24Prt1 { get; set; }

            [JsonIgnore]
            public string subPhrs12_24Prt2 { get; set; }

            [JsonIgnore]
            public decimal thundr12_24__thndrEnum { get; set; }

            [JsonIgnore]
            public string thndr12_24 { get; set; }

            [JsonIgnore]
            public string _sunriseISOLocal { get; set; }

            [JsonIgnore]
            public string _sunsetISOLocal { get; set; }

            [JsonIgnore]
            public string _moonRiseISOLocal { get; set; }

            [JsonIgnore]
            public string _moonSetISOLocal { get; set; }

            [JsonIgnore]
            public string fcstDateTimeISO { get; set; }

            [JsonIgnore]
            public string sunriseISO { get; set; }

            [JsonIgnore]
            public string sunsetISO { get; set; }

            [JsonIgnore]
            public string moonRiseISO { get; set; }

            [JsonIgnore]
            public string moonSetISO { get; set; }

            [JsonIgnore]
            public decimal sky { get; set; }

            [JsonIgnore]
            public decimal skyX { get; set; }

            [JsonIgnore]
            public string snsblWx { get; set; }

            [JsonIgnore]
            public string tSnsblWx { get; set; }

            [JsonIgnore]
            public string wrlsWx { get; set; }

            /// <summary>
            /// 白天降雨概率，格式：20
            /// </summary>
            [JsonProperty(Alias = "pOP24")]
            public decimal ChanceOfRainOfDay { get; set; }

            [JsonIgnore]
            public string precipTyp24 { get; set; }

            [JsonIgnore]
            public string altDyPrtNm { get; set; }

            [JsonIgnore]
            public string wxMan24 { get; set; }

            [JsonIgnore]
            public decimal sky12 { get; set; }

            [JsonIgnore]
            public decimal sky12X { get; set; }

            /// <summary>
            /// 白天天气条件，格式："Sunny"
            /// </summary>
            [JsonProperty(Alias = "snsblWx12")]
            public string DayWeatherCondition { get; set; }

            [JsonIgnore]
            public string tSnsblWx12 { get; set; }

            [JsonIgnore]
            public string wrlsWx12 { get; set; }

            [JsonIgnore]
            public decimal pOP12 { get; set; }

            [JsonIgnore]
            public decimal rH12 { get; set; }

            [JsonIgnore]
            public decimal hIF { get; set; }

            [JsonIgnore]
            public decimal hIC { get; set; }

            [JsonIgnore]
            public decimal wCF12 { get; set; }

            [JsonIgnore]
            public decimal wCC12 { get; set; }

            [JsonIgnore]
            public decimal wSpdM12 { get; set; }

            [JsonIgnore]
            public decimal wSpdK12 { get; set; }

            [JsonIgnore]
            public decimal wSpdKn12 { get; set; }

            [JsonIgnore]
            public decimal wDir12 { get; set; }

            [JsonIgnore]
            public string wDirAsc12 { get; set; }

            [JsonIgnore]
            public string _wDirAsc12_en { get; set; }

            [JsonIgnore]
            public decimal clds12 { get; set; }

            [JsonIgnore]
            public decimal uvIdx { get; set; }

            [JsonIgnore]
            public decimal uvIdxRw { get; set; }

            [JsonIgnore]
            public string uvDes { get; set; }

            [JsonIgnore]
            public decimal uvWrn { get; set; }

            [JsonIgnore]
            public decimal glfIdx { get; set; }

            [JsonIgnore]
            public string glfCat { get; set; }

            [JsonIgnore]
            public decimal qpf12 { get; set; }

            [JsonIgnore]
            public decimal snwQpf12 { get; set; }

            [JsonIgnore]
            public string subPhrs24Prt1 { get; set; }

            [JsonIgnore]
            public string precipTyp12 { get; set; }

            [JsonIgnore]
            public string subPhrs12Prt1 { get; set; }

            [JsonIgnore]
            public string wxMan12 { get; set; }

            [JsonIgnore]
            public decimal thndr12_thndrEnum { get; set; }

            [JsonIgnore]
            public string thndr12 { get; set; } 
        }
    }
}
