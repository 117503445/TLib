using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TLib.IO
{
    public class UsbCopyer
    {

        /// <summary>
        /// 例:  "G:/",初始化为
        /// </summary>
        private string hackDrive = DriveInfo.GetDrives().Last().DriveType == DriveType.Removable ? DriveInfo.GetDrives().Last().Name : "";
        /// <summary>
        /// 不点击托盘,直接进行copy
        /// </summary>
        private bool isDirectCopy = false;
        public bool IsDirectCopy
        {
            get => isDirectCopy; set
            {
                isDirectCopy = value;

            }
        }
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
                UsbWatcher watcher = new UsbWatcher();
                watcher.UsbDiskEnter += UsbDiskEnter;
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
                try
                {
                    Console.WriteLine("Coying:HackDrive={0},Path={1}", dirSource, dirDestination + timestamp);
                    TIO.CopyFolder(dirSource, dirDestination + timestamp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Console.WriteLine("Copy:{0} finished", timestamp);
            });
        }

        private void CopyUSB()
        {
            CopyUSB(hackDrive, dirBackup);
        }

    }
    public class UsbDiskEnterEventArgs : EventArgs
    {
        public UsbDiskEnterEventArgs(DriveInfo drive) { Drive = drive; Console.WriteLine($"UsbDiskEnter:{drive}"); }
        public DriveInfo Drive { get; }
    }
    public class UsbWatcher
    {

        private DriveInfo[] lastDrives = DriveInfo.GetDrives();
        public UsbWatcher()
        {
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        public event EventHandler<UsbDiskEnterEventArgs> UsbDiskEnter;
        private void Timer_Tick(object sender, EventArgs e)
        {
            var s = DriveInfo.GetDrives();
            if (s.Length > lastDrives.Length && s.Last().DriveType == DriveType.Removable)
            {
                UsbDiskEnter(sender, new UsbDiskEnterEventArgs(s.Last()));
            }
            lastDrives = s;
        }
    }
}
