using System;
using DataModels;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DBAccess
{
    class Program
    {

        //static IPersonDao dao = new PersonLINQDao();
        static IPersonDao dao;
        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            try
            {
                var token = appSettings["InputToken"] ?? ",";
                var headerString = appSettings["Header"] ?? string.Empty;
                var interval = appSettings["IntervalDay"] ?? "7";
                var sequenceFormat = appSettings["InputSequence"] ?? "ID,Name,Sex (0: Female, 1: Male),Birthday (ex:2000/10/10)";
                var connectString = appSettings["DevelopmentDB"] ??  "Server = 127.0.0.1; Port = 5432; Database = mydatabase; User Id = myid; Password = mypassword;Pooling=true;MinPoolSize=10;MaxPoolSize=100;" ;
                IList<Person> exportList = new List<Person>();

                Debug.Assert(!string.IsNullOrWhiteSpace(token));
                Debug.Assert(!string.IsNullOrWhiteSpace(headerString));
                Debug.Assert(!string.IsNullOrWhiteSpace(sequenceFormat));

                dao = new PersonCmdDao(connectString);

                int result;
                var dayInterval = int.TryParse(interval, out result) ? result : 7;

                var curMode = UserMode.Exit;
                var preMode= UserMode.Exit;
                var mode = -1;
                do
                {
                    if (curMode != UserMode.Contiune)
                    {
                        Console.WriteLine("Please enter the mode you want to proceed,  ");
                        Console.Write("[1]: ExportCSV2DB, [2]: QueryBirthday(1 week berfore/after till now), Others: Exit): ");
                        var input = Console.ReadLine();
                        mode = int.TryParse(input, out mode) ? mode : (int)UserMode.Exit;

                        if (!Enum.IsDefined(typeof(UserMode), mode))
                        {
                            curMode = UserMode.Exit;
                        }
                        else
                        { 
                            curMode = (UserMode)Enum.ToObject(typeof(UserMode), mode);
                        }
                    }
                    else
                    {
                        curMode = preMode;
                    }

                    switch(mode)
                    {
                        case (int)UserMode.ExportCCSV2DB:
                            Console.WriteLine($"Please enter the format in sequence,{{{sequenceFormat}}}.");
                            Console.WriteLine($"Note you should sperate each column in token '{token}'");
                            var inString = Console.ReadLine();
                            string[] data = inString.Split(Convert.ToChar(token));
                            string[] headers = headerString.Split(Convert.ToChar(token));

                            var person = ValidateCSVInput(data, headers);
                            if (person != null)                               
                            {
                                // add to the export list
                                exportList.Add(person);
                            }

                            Console.Write("Want to import next reocrd (Y/N): ");
                            var ans = Console.ReadLine();
                            var goNext = string.Compare(ans, "Y", true) == 0 ? true : false;
                            if (goNext)
                            {
                                curMode = UserMode.Contiune;
                                preMode = UserMode.ExportCCSV2DB;
                            }
                            else
                            {
                                // dump list to database
                                Console.WriteLine("Start to dump input data to database, please wait...");
                                Task dumpTask = new  Task(() => DumpData2DB(exportList));
                                dumpTask.Start();
                                dumpTask.Wait();

                                exportList.Clear();
                            }

                            break;
                        case (int)UserMode.QueryBirthday:
                            Console.WriteLine("Start to query from database, please wait...");

                            Task queryTask = new Task(() => QueryBirthday(dayInterval, headerString));
                            queryTask.Start();
                            queryTask.Wait();

                            break;
                        case (int)UserMode.Exit:
                            curMode = UserMode.Exit;
                            break;
                    }

                } while (curMode != UserMode.Exit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error message: {0}", "StackTrace: {1}"), ex.Message, ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("-------------");
                Console.WriteLine("Please press any key to exit.");
                Console.ReadKey();
            }
        }
        
        private static void QueryBirthday(int dayInterval, string headerString)
        {
            try
            {
                var persons = dao.GetPersonByBirthDateTimeRange(DateTime.Now, dayInterval);

                if(persons.Count == 0)
                {
                    Console.WriteLine("Sorry, we could not found any result");
                    return;
                }

                var header = headerString.Replace(',', '|');
                Console.WriteLine($"|{header}|");
                foreach(var person in persons)
                {
                    Console.WriteLine(person.ToOutputString());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Error message: {0}", "StackTrace: {1}"), ex.Message, ex.StackTrace);
            }
        }

        private static void DumpData2DB(IList<Person> exportList)
        {
            try
            {
                foreach (var person in exportList)
                {
                    //var id = person.Id;
                    var act = dao.InsertOrUpdate(person);
                    if (act == DBActions.Insert)
                    {
                        Console.WriteLine($"Insert record: ID: {person.Id}, Name: {person.Name}");
                    }
                    else if (act == DBActions.Update)
                    {
                        Console.WriteLine($"Update record: ID: {person.Id}, Name: {person.Name}");
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Error message: {0}", "StackTrace: {1}"), ex.Message, ex.StackTrace);
            }


        }

        private static Person ValidateCSVInput(string[] data, string[] headers)
        {
            if (data.Length != headers.Length)
            {
                Console.WriteLine("The input data format were unexpected, please follow the input rule");
                return null;
            }

            // check id
            int id = -1;
            if (!int.TryParse(data[0], out id))
            {
                Console.WriteLine("Invaild [ID] input, please try again");
                return null;
            }

            // check name
            var name = data[1];
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Invaild [Name] input, please try again");
                return null;
            }

            Sex sex;
            if (!Enum.TryParse(data[2], out sex) || (sex != Sex.Female && sex != Sex.Male))
            {
                Console.WriteLine("Invaild [Sex] input, please try again");
                return null;
            }

            // check birthday
            DateTime birthday;
            if (!DateTime.TryParse(data[3], out birthday))
            {
                Console.WriteLine("Invaild [Birthday] input, please try again");
                return null;
            }

            return new Person()
            {
                Id = id,
                Name = name,
                Sex = Extension.TransferSex2Display(sex),
                Birthday = birthday
            };
        }
    }

    public enum UserMode
    {
        ExportCCSV2DB = 1,
        QueryBirthday,
        Exit,
        Contiune = 100
    }
}
