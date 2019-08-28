using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLib.IO
{
    public class UsbCopyer
    {

        /// <summary>
        /// 例:  "G:/",初始化为
        /// </summary>
        private string hackDrive = DriveInfo.GetDrives().Last().DriveType == DriveType.Removable ? DriveInfo.GetDrives().Last().Name : string.Empty;

        public bool IsDirectCopy { get; set; } = false;
        private readonly string dirBackup = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirBackup">备份路径,Exp:"D:/temp/"</param>
        /// <param name="isDirectCopy">直接拷贝模式</param>
        public UsbCopyer(string dirBackup, bool isDirectCopy)
        {
            this.dirBackup = dirBackup;
            IsDirectCopy = isDirectCopy;
            if (IsDirectCopy)
            {
                UsbWatch.Start();
                UsbWatch.UsbDiskEnter += UsbDiskEnter;
            }
        }
        private void UsbDiskEnter(object sender, UsbDiskEnterEventArgs e)
        {
            hackDrive = e.Drive.Name;
            CopyUSB();
        }
        /// <summary>
        /// 封装后的拷贝
        /// </summary>
        /// <param name="dirSource">源文件夹路径</param>
        /// <param name="dirDestination">目标文件夹路径</param>
        private static void CopyUSB(string dirSource, string dirDestination)
        {
            Task.Run(() =>
            {
                string timestamp = Software.TimeStamp.Now;
                Console.WriteLine($"Coying:HackDrive={dirSource},Path={dirDestination + timestamp}");
                TFile.CopyFolder(dirSource, dirDestination + timestamp);
                Console.WriteLine($"Copy:{timestamp} finished");
            });
        }

        private void CopyUSB()
        {
            CopyUSB(hackDrive, dirBackup);
        }

    }
}
