using APIGET.WeacherViewer.Misc;
using APIGET.WeacherViewer.Models;
using APIGET.Weather.Core;
using APIGET.Weather.Core.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace APIGET.WeatherViewer.Controllers
{
  
    [ErrorHandlerExtAttr]
    public class HomeController : Controller
    {
        public static List<City> CityAvaiable = new List<City>();

        // GET: Home
        public ActionResult Index()
        {
            if(CityAvaiable.Count == 0)
            {
                var feeds = LivedoorWeatherProvider.Instance.RetrieveAvailableCityId();
                CityAvaiable.AddRange(feeds);
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForecastResult(string jsonSearchCriteria)
        {
            var searchCriteria = JsonConvert.DeserializeObject<WeatherSearchCriteria>(jsonSearchCriteria);

            var whereTerms = searchCriteria.CityIds.SelectMany(id => searchCriteria.ForecastDate.Select(date => $"('{id}','{date}')"));

            // query to database
            var citys = await LivedoorWeatherProvider.Instance.FindWeatherInfos(whereTerms);
          
            var model = new WeatherResultViewModel()
            {
                Result = citys.OrderBy(c => c.Date).ThenBy(c => c.CityName) // sort by date then city name
            };

            return View("Result", model);
        }

        [HttpPost]
        public JsonResult FilterCitys(string prefs)
        {
            var criteria = prefs.Split(',');

            var citys = from city in CityAvaiable
                        join pref in criteria
                        on city.Prefecture equals pref
                        select city;

            if(citys.Count() > 0)
            {
                return Json(citys);
            }

            return Json(null);
        }
    }
}