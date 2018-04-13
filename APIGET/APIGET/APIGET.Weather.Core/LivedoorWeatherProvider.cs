
using APIGET.Weather.Core.Database;
using Common.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace APIGET.Weather.Core
{

    public class LivedoorWeatherProvider 
    {
        private static readonly Lazy<LivedoorWeatherProvider> lazy =
        new Lazy<LivedoorWeatherProvider>(() => new LivedoorWeatherProvider());

        private static readonly ILog Log = LogManager.GetLogger<LivedoorWeatherProvider> ();

        public static LivedoorWeatherProvider Instance { get { return lazy.Value; } }

        private ConcurrentStack<City> weatherDatas;
        public ConcurrentStack<City> WeatherDatas
        {
            get
            {
                return weatherDatas;
            }

            set
            {
                weatherDatas = value;
            }
        }

        private CityDao dao;

        readonly string feedUrl = "http://weather.livedoor.com/forecast/rss/primary_area.xml";
        readonly string webServiceUrlbase = "http://weather.livedoor.com/forecast/webservice/json/v1?city=";

        private LivedoorWeatherProvider()
        {
            weatherDatas = new ConcurrentStack<City>();
            dao = new CityDao();
        }

        public async Task<IEnumerable<City>> FindWeatherInfos(IEnumerable<string> wheres)
        {
            var ret = await dao.GetWeatherInfos(wheres);
            return ret;
        }

        public void StoreForecastDatas(IEnumerable<City> citys)
        {
            Parallel.ForEach(citys, (city, loopState) =>
           {
               try
               {
                   dao.InsertOnConfilct(city);
               }
               catch (Exception ex)
               {
                   Log.Error($"Error occured, message: {ex.Message}, strack trace: {ex.StackTrace}");
               }
           });
        }

        public async Task GetWeatherForecast(string feedUri = "", string webServiceUri = "")
        {
            feedUri = feedUri != string.Empty ? feedUri : this.feedUrl;
            webServiceUri = webServiceUri != string.Empty ? webServiceUri : webServiceUrlbase;

            var availableCitys = RetrieveAvailableCityId(feedUri);

            await RetrieveCityWeatherInformation(availableCitys, webServiceUri);
        }

        public IEnumerable<City> RetrieveAvailableCityId(string targetUri = "")
        {
            targetUri = targetUri != string.Empty ? targetUri : feedUrl;
            // TODO, move the hard code tag  to configure file
            var xml = XElement.Load(targetUri);
           
            var ret = xml.DescendantsAndSelf("city").Select(e => new City()
            {
                CityName = e.Attribute("title").Value,
                CityId = e.Attribute("id").Value,
                Prefecture = e.Parent.Attribute("title").Value
            });
            return ret;
        }

        // TODO: make the hard code tag to configure file, maybe some day structure change, code should need not change?
        public async Task RetrieveCityWeatherInformation(IEnumerable<City> citys, string baseUri)
        {
            // clear previous result
            weatherDatas.Clear();

            await Extension.ForEachAsync(citys, async (city) =>
            {
                try
                {
                    var requestUri = baseUri + city.CityId;
                    using (var client = new HttpClient())
                    {
                        var result = await client.GetStringAsync(requestUri);

                        var dynamicObj = JObject.Parse(result);
                        // using json path
                        var forecasts = dynamicObj.SelectTokens("$..forecasts").Children().ToList();

                        foreach (var forecast in forecasts)
                        {
                            City _city = new City();
                            _city.CityId = city.CityId;
                            _city.CityName = city.CityName;
                            _city.Date = forecast.Value<DateTime>("date");
                            _city.WeatherDescription = forecast.Value<string>("telop");

                            var temperatureMax = forecast.SelectToken("temperature.max");
                            if (temperatureMax.HasValues)
                            {
                                _city.MaxTemperatureCelsius = temperatureMax.Value<int>("celsius");
                            }

                            var temperatureMin = forecast.SelectToken("temperature.min");
                            if (temperatureMin.HasValues)
                            {
                                _city.MinTemperatureCelsius = temperatureMin.Value<int>("celsius");
                            }

                            weatherDatas.Push(_city);
                        }

                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.StackTrace);
                    Log.Error($"Error occured, message: {ex.Message}, strack trace: {ex.StackTrace}");
                }
            });

          
           
        }

    }
}
