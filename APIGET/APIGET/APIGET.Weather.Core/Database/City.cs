using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace APIGET.Weather.Core.Database
{
    [Serializable]
    public class City
    {
        [JsonIgnore]
        [ScriptIgnore]
        public int Id { get; set; }
        public string CityName { get; set; }
        public string CityId { get; set; }
        public DateTime Date { get; set; }
        public string WeatherDescription { get; set; }
        public int? MaxTemperatureCelsius { get; set; }
        public int? MinTemperatureCelsius { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public DateTime InsertTime { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public DateTime UpdateTime { get; set; }

        public string Prefecture { get; set; }

        public string OutputForecastDate
        {
            get
            {
                return $"{Date.Year}年{Date.Month}月{Date.Day}日";
            }
        }

        public string OutputTemperateCelsius
        {
            get
            {
                int? temperature = 0;
                
                if(Date.Month >= 6 && Date.Month <= 9)
                {
                    // asume that summer always care about the max temperature
                    temperature = MaxTemperatureCelsius.HasValue ? MaxTemperatureCelsius :
                     MinTemperatureCelsius.HasValue ? MinTemperatureCelsius : null;
                }
                else
                {
                    // asume others seans use the min one(if we have)
                    temperature = MinTemperatureCelsius.HasValue ? MinTemperatureCelsius :
                 MaxTemperatureCelsius.HasValue ? MaxTemperatureCelsius : null;
                }

                return temperature.HasValue ? temperature.ToString() + "℃ " : "N/A";
                    
            }
        }
    }
}
