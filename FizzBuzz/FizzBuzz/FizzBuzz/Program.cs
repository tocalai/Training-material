using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzz
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var start = 1;
                var input = args[0];
                int.TryParse(input, out int end);
                var outMessage = string.Empty;
                var enumerator = new FizzBuzzEnumerator(start, end);

                while(enumerator.MoveNext())
                {
                    outMessage = enumerator.Current;
                    Console.WriteLine(outMessage);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}", "StackTrace: {ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine("-------------");
                Console.WriteLine("Please press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
