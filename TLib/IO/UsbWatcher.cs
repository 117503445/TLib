using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace TLib.IO
{
    /// <summary>
    /// 监视磁盘变化
    /// </summary>
    public static class UsbWatch
    {

        private static DriveInfo[] lastDrives = DriveInfo.GetDrives();
        private static DispatcherTimer timer;
        private static int spanMilliseconds = 1000;
        /// <summary>
        /// 扫描磁盘变化的时间间隔
        /// </summary>
        public static int SpanMilliseconds
        {
            get => spanMilliseconds;
            set
            {
                spanMilliseconds = value;
                if (timer != null)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(SpanMilliseconds);
                }
            }
        }
        /// <summary>
        /// 开始监视磁盘变化
        /// </summary>
        public static void Start()
        {
            if (timer is null)
            {
                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(SpanMilliseconds)
                };
            }
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        /// <summary>
        /// 停止监视磁盘变化
        /// </summary>
        public static void Stop()
        {
            timer.Stop();
        }
        public static event EventHandler<UsbDiskEnterEventArgs> UsbDiskEnter;
        private static void Timer_Tick(object sender, EventArgs e)
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
