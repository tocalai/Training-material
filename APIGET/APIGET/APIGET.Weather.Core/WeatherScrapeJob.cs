using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIGET.Weather.Core
{
    [DisallowConcurrentExecution]
    public class WeatherScrapeJob : IJob
    {
        private static readonly ILog Log = LogManager.GetLogger<WeatherScrapeJob>();

        // TODO: insert the weahter information according the city + date
        //             this may cause the history data growing in database, need patch(or delete) the oldest/not useful weather data in db
        public void Execute(IJobExecutionContext context)
        {
            Task.Factory.StartNew(() =>
            {
                Log.Info("Start GetWeatherForecast()...");
                LivedoorWeatherProvider.Instance.GetWeatherForecast().Wait();

                Log.Info($"Completed GetWeatherForecast(), total ({LivedoorWeatherProvider.Instance.WeatherDatas.Count}) record returns");
            }).ContinueWith(task =>
            {
               
                Log.Info("Start StoreForecastDatas()...");
                LivedoorWeatherProvider.Instance.StoreForecastDatas(LivedoorWeatherProvider.Instance.WeatherDatas);
                Log.Info("Completed StoreForecastDatas()...");
            });
        }
    }
}
