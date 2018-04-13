using APIGET.Weather.Core;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading;

namespace APIGET.WeatherWatcher
{
    class Program
    {
        private static ILog Log = LogManager.GetLogger<Program>();

        static void Main(string[] args)
        {        
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(exceptionHandler);

            Console.TreatControlCAsInput = true;

            // TODD: move schedule setting data to configure file to make it flexible
            var scheduleFactory = new StdSchedulerFactory();
            var schedular = scheduleFactory.GetScheduler();

            var job = JobBuilder.Create<WeatherScrapeJob>()
                .WithIdentity(typeof(WeatherScrapeJob).GetType().Name)
                .StoreDurably()
                .Build();

            var simpleSchedule = SimpleScheduleBuilder.Create();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSchedule(simpleSchedule.WithIntervalInMinutes(10).RepeatForever())
                //.WithCronSchedule("0 0/10 * * * ?")// every 10 mins execute the job 
                .WithIdentity(typeof(WeatherScrapeJob).GetType().Name + "Tr")
                .Build();
        
            schedular.ScheduleJob(job, trigger);

            schedular.Start();

            Console.WriteLine("Press [Escape (Esc)] to exit the tesing running of scrape weather job");
           
            ConsoleKeyInfo cki;
           do
            {
                cki = Console.ReadKey();
                // do nothing
                // blocking here
                Thread.Sleep(50);
            }while(cki.Key != ConsoleKey.Escape);

            Console.WriteLine("Waiting job shut down...");
            schedular.Shutdown(true);

            Console.WriteLine("Job shut down competed,, please press any key to exit");
            Console.ReadKey();

        }

        private static void exceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Error($"Unhandled exception, message: {ex.Message}, stack trace: {ex.StackTrace}");
        }
    }
}
