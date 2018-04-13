using APIGET.Weather.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIGET.WeacherViewer.Models
{
    public class WeatherResultViewModel
    {
        public IEnumerable<City> Result { get; set; }
    }
}