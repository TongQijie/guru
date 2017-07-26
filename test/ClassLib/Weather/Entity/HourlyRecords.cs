namespace ClassLib.Weather.Entity
{
    using Guru.Formatter.Json;

    public class HourlyRecords
    {
        [JsonIgnore]
        public DHHdrObject DHHdr { get; set; }

        public DHDataObject[] DHData { get; set; }

        public class DHHdrObject
        {
            [JsonIgnore]
            public string coopId { get; set; }

            [JsonIgnore]
            public string stnNm { get; set; }

            [JsonIgnore]
            public string procTm { get; set; }

            [JsonIgnore]
            public string _procTmLocal { get; set; }

            [JsonIgnore]
            public string procTmISO { get; set; }
        }

        public class DHDataObject
        {
            [JsonIgnore]
            public decimal hrNum { get; set; }

            [JsonIgnore]
            public string dow { get; set; }

            [JsonIgnore]
            public decimal fcstValGMT { get; set; }

            [JsonIgnore]
            public string DHlclValTm { get; set; }

            [JsonIgnore]
            public string fcstValDay { get; set; }

            [JsonIgnore]
            public string fcstValTm { get; set; }

            /// <summary>
            /// 日期，格式："20160202"
            /// </summary>
            [JsonProperty(Alias = "locValDay")]
            public string Date { get; set; }

            /// <summary>
            /// 时间，格式："220000"
            /// </summary>
            [JsonProperty(Alias = "locValTm")]
            public string Time { get; set; }

            [JsonIgnore]
            public decimal tmpF { get; set; }

            /// <summary>
            /// 温度，格式：1
            /// </summary>
            [JsonProperty(Alias = "tmpC")]
            public decimal Temperature { get; set; }

            [JsonIgnore]
            public decimal sky { get; set; }

            [JsonIgnore]
            public decimal skyX { get; set; }

            /// <summary>
            /// 天气情况，格式："Clear"
            /// </summary>
            [JsonProperty(Alias = "snsblWx")]
            public string WeatherCondition { get; set; }

            [JsonIgnore]
            public string tSnsblWx { get; set; }

            [JsonIgnore]
            public string wrlsWx { get; set; }

            /// <summary>
            /// 降雨概率，格式：10
            /// </summary>
            [JsonProperty(Alias = "pOP")]
            public decimal ChanceOfRain { get; set; }

            /// <summary>
            /// 湿度，格式：78
            /// </summary>
            [JsonProperty(Alias = "rH")]
            public decimal Humidity { get; set; }

            [JsonIgnore]
            public decimal wSpdM { get; set; }

            [JsonIgnore]
            public decimal wSpdK { get; set; }

            [JsonIgnore]
            public decimal wSpdKn { get; set; }

            [JsonIgnore]
            public decimal wDir { get; set; }

            [JsonIgnore]
            public string wDirAsc { get; set; }

            [JsonIgnore]
            public string _wDirAsc_en { get; set; }

            [JsonIgnore]
            public decimal hIF { get; set; }

            [JsonIgnore]
            public decimal hIC { get; set; }

            [JsonIgnore]
            public decimal wCF { get; set; }

            [JsonIgnore]
            public decimal wCC { get; set; }

            [JsonIgnore]
            public decimal visM { get; set; }

            [JsonIgnore]
            public decimal visK { get; set; }

            [JsonIgnore]
            public decimal feelsLikeF { get; set; }

            [JsonIgnore]
            public decimal feelsLikeC { get; set; }

            [JsonIgnore]
            public decimal qpf { get; set; }

            [JsonIgnore]
            public decimal qpfMm { get; set; }

            [JsonIgnore]
            public decimal snowQpf { get; set; }

            [JsonIgnore]
            public decimal snowQpfMm { get; set; }

            [JsonIgnore]
            public string severity { get; set; }

            [JsonIgnore]
            public decimal mslp { get; set; }

            [JsonIgnore]
            public decimal mslpMb { get; set; }

            [JsonIgnore]
            public decimal clds { get; set; }

            [JsonIgnore]
            public decimal dwptF { get; set; }

            [JsonIgnore]
            public decimal dwptC { get; set; }

            [JsonIgnore]
            public decimal uvIdx { get; set; }

            [JsonIgnore]
            public decimal uvIdxRw { get; set; }

            [JsonIgnore]
            public string uvDes { get; set; }

            [JsonIgnore]
            public decimal uvWrn { get; set; }

            [JsonIgnore]
            public string glfCat { get; set; }

            [JsonIgnore]
            public string precipTyp { get; set; }

            [JsonIgnore]
            public string dyNghtnd { get; set; }

            [JsonIgnore]
            public string wxMan { get; set; }

            [JsonIgnore]
            public string subPhrsPrt1 { get; set; }

            [JsonIgnore]
            public string subPhrsPrt2 { get; set; }

            [JsonIgnore]
            public string fcstDateTimeISO { get; set; }

            [JsonIgnore]
            public string fcstDateTimeISOLocal { get; set; }
        }
    }
}
