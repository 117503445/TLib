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
            Logger.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.Read();
        }
    }
}
