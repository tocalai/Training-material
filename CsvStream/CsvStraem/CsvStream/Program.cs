using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;

namespace CsvStrem
{
    class Program
    {
        static readonly int MAX = 2048;
        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
                        
            try
            {
                var exportPath = appSettings["ExportPath"] ?? string.Empty;
                var exportFileName = appSettings["ExportName"] ?? string.Empty;
                var token = appSettings["InputToken"] ?? ",";
                var headerString = appSettings["CSVHeader"] ?? string.Empty;
                var sequenceFormat = appSettings["InputSequence"] ?? string.Empty;
                IList<Person> exportList = new List<Person>();
                ConcurrentStack<Person> csvFileList = new ConcurrentStack<Person>();


                Debug.Assert(!string.IsNullOrWhiteSpace(exportPath));
                Debug.Assert(!string.IsNullOrWhiteSpace(exportFileName));
                Debug.Assert(!string.IsNullOrWhiteSpace(token));
                Debug.Assert(!string.IsNullOrWhiteSpace(headerString));
                
                var curStatus = Status.Exit;
                var preStatus = Status.Exit;

                Console.WriteLine("Load CSV file to memory...");
                // load CSV file
                Task loadTask = new Task(() => LoadDataFromCSVFile(exportPath, exportFileName, csvFileList));
                loadTask.Start();
                loadTask.Wait();

                do
                {
                    if (curStatus != Status.Contiune)
                    {
                        Console.Write("Please enter the mode you want to proceed (0: ExportToCSVFile, 1: ImportFromCSV, Others: Exit): ");
                        var mode = Console.ReadLine();
                        curStatus = GetProceedMode(mode);
                    }
                    else
                    {
                        curStatus = preStatus;
                    }

                    switch(curStatus)
                    {
                        case Status.ExportCSV:
                            Console.WriteLine($"Please enter the format in sequence, {{{sequenceFormat}}}.");
                            Console.WriteLine($"Note you should sperate each column in token '{token}'");
                            var inString = Console.ReadLine();
                            string[] data = inString.Split(Convert.ToChar(token));
                            string[] headers = headerString.Split(Convert.ToChar(token));

                            var person = ValidateCSVInput(data, headers, exportList, csvFileList);
                            if( person != null)
                            {
                                // add to the export list
                                exportList.Add(person);
                            }

                            Console.Write("Want to export next reocrd (Y/N): ");
                            var ans = Console.ReadLine();
                            var goNext = string.Compare(ans, "Y", true) == 0 ? true : false;
                            if(goNext)
                            {
                                curStatus = Status.Contiune;
                                preStatus = Status.ExportCSV;
                            }
                            else
                            {
                                // write data to CSV file
                                Task exportTask = new Task( () => ExportDataToCSV(exportList, exportPath, exportFileName, headerString));
                                exportTask.Start();
                                exportTask.Wait();
                                exportList.Clear();// reset the list

                                Console.WriteLine("Load CSV file to memory...");
                                loadTask = new Task(() => LoadDataFromCSVFile(exportPath, exportFileName, csvFileList));
                                loadTask.Start();
                                loadTask.Wait();
                            }

                            break;
                        case Status.ImportCSV:
                            Console.WriteLine("Trying to import file to console, please wait...");

                            Task importTask = new Task(() => ImportDataFromCSV(exportPath, exportFileName, headerString, csvFileList));
                            importTask.Start();
                            importTask.Wait();

                            break;
                        case Status.Exit:
                            break;
                    }

                } while (curStatus != Status.Exit);
            }
            catch(Exception ex)
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

        private static void ImportDataFromCSV(string exportPath, string exportFileName, string headerText, ConcurrentStack<Person> csvFileList)
        {
            try
            {
                var targetPath = Path.Combine(Environment.CurrentDirectory, exportPath);
                var fullFilePath = Path.Combine(targetPath, exportFileName) + ".csv";

                if (File.Exists(fullFilePath))
                {
                    if (new FileInfo(fullFilePath).Length == 0)
                    {
                        Console.WriteLine(string.Format("Target file: {0} without content inside(is empty)", fullFilePath));
                        return;
                    }

                    headerText = headerText.Replace(',', '|');
                    Console.WriteLine($"|{headerText}|");
                    var sortList = csvFileList.OrderBy(p => p.ID);
                    var allLines = string.Join(Environment.NewLine, sortList.Select(s => s.ToOutputString()));
                    Console.WriteLine(allLines);


                }
                else
                {
                    Console.WriteLine(string.Format("Target file: {0} not existed", fullFilePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error message: {0}", "StackTrace: {1}"), ex.Message, ex.StackTrace);
            }
        }

        private static void LoadDataFromCSVFile(string exportPath, string exportFileName, ConcurrentStack<Person> csvFileList)
        {
            try
            {
                csvFileList.Clear();
                var targetPath = Path.Combine(Environment.CurrentDirectory, exportPath);
                var fullFilePath = Path.Combine(targetPath, exportFileName) + ".csv";

                if (!File.Exists(fullFilePath))
                {
                    return; // do nothing
                }

                string[] allLines = new string[MAX];
                allLines = File.ReadAllLines(fullFilePath);

                Parallel.For(0, allLines.Length, index =>
                {
                    if(index == 0 || string.IsNullOrWhiteSpace(allLines[index]))
                    {
                        return;
                    }
                    string[] data = allLines[index].Split(',');

                     var person = new Person()
                     {
                         ID = int.Parse(data[0]),
                         Name = data[1],
                         Sex = (Sex)Enum.Parse(typeof(Sex), Extension.SexDic[data[2]], true),
                         Birthday = DateTime.Parse(data[3])
                     };

                    csvFileList.Push(person);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error message: {0}", "StackTrace: {1}"), ex.Message, ex.StackTrace);
            }
        }
        private static void ExportDataToCSV(IList<Person> exportList, string exportPath, string exportFileName, string headerText)
        {
            try
            {
                var targetPath = Path.Combine(Environment.CurrentDirectory, exportPath);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                var fullFilePath = Path.Combine(targetPath, exportFileName) + ".csv";
                var isExisted = File.Exists(fullFilePath);
                using (var streamWriter = new StreamWriter(fullFilePath, isExisted))
                {
                    if(!isExisted)
                    {
                        // write header text
                        streamWriter.WriteLine(headerText);
                    }

                    // write data text
                    var allDataText = string.Join(Environment.NewLine, exportList.Select(i => i.ToCSVString()));
                    streamWriter.WriteLine(allDataText);


                }

                Console.WriteLine(string.Format("Total ({0}) record export to file: {1}", exportList.Count, fullFilePath));
              
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error message: {0}", "StackTrace: {1}"), ex.Message, ex.StackTrace);
            }
        }

        private static Person ValidateCSVInput(string[] data, string[] headers, IList<Person> exportList, ConcurrentStack<Person> csvFileList)
        {
           
            if(data.Length != headers.Length)
            {
                Console.WriteLine("The input data format were unexpected, please follow the input rule");
                return null;
            }

            // check id
            int id = -1;
            if(!int.TryParse(data[0], out id))
            {
                Console.WriteLine("Invaild [ID] input, please try again");
                return null;
            }

            if(exportList.Any(p => p.ID == id) || csvFileList.Any(p => p.ID == id))
            {
                Console.WriteLine("Duplicated [ID] input, please try again");
                return null;
            }

            // check name
            var name = data[1];
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Invaild [Name] input, please try again");
                return null;
            }

            // check sex
            Sex sex;
            if(!Enum.TryParse(data[2], out sex) || (sex  != Sex.Female && sex != Sex.Male) )
            {
                Console.WriteLine("Invaild [Sex] input, please try again");
                return null;
            }

            // check birthday
            if (!DateTime.TryParse(data[3], out DateTime birthday))
            {
                Console.WriteLine("Invaild [Birthday] input, please try again");
                return null;
            }

            return new Person()
            {
                ID = id,
                Name = name,
                Sex = sex,
                Birthday = birthday
            };
        }

        private static Status GetProceedMode(string userInput)
        {
            var mode = Status.Exit;
            int result = -1;
            if(int.TryParse(userInput, out result))
            {
                switch(result)
                {
                    case (int)Status.ExportCSV:
                        mode = Status.ExportCSV;
                        break;
                    case (int)Status.ImportCSV:
                        mode = Status.ImportCSV;
                        break;
                }
            }

            return mode;
        }

        public enum Status
        {
            ExportCSV = 0,
            ImportCSV = 1,
            Exit,
            Contiune = 100
        }
    }
}
