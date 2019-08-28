using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLib.IO;
namespace ConsoleTest
{
    internal static class Program
    {
#pragma warning disable IDE0060 // 删除未使用的参数
        private static void Main(string[] args)
#pragma warning restore IDE0060 // 删除未使用的参数
        {
            int testIndex = 0;
            Console.WriteLine($"{nameof(testIndex)} = {testIndex}");
            Test(testIndex);
            Console.Read();
        }
        private static void Test(int n)
        {
            switch (n)
            {
                case 0:
                    UsbWatcherTest();
                    break;
                default:
                    break;
            }
        }
        private static void UsbWatcherTest()
        {
            UsbWatch.Start();
            UsbWatch.UsbDiskEnter += UsbWatch_UsbDiskEnter;
        }

        private static void UsbWatch_UsbDiskEnter(object sender, UsbDiskEnterEventArgs e)
        {
            Console.WriteLine(e.Drive);
        }
    }
}
