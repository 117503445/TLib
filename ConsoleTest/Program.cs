using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLib.Software;
namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.WriteLine("Hello World");
            try
            {
                int a = 0;
                int b = 1 / a;
            }
            catch (Exception ex)
            {

                Logger.WriteException(ex);
            }
            Console.Read();
        }
    }
}
