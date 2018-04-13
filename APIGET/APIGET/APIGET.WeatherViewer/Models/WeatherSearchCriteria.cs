using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIGET.WeacherViewer.Models
{
    public class WeatherSearchCriteria
    {
        public string[] CityIds { get; set; }
        public string[] ForecastDate { get; set; }
    }
}