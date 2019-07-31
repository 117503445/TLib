using System;
using System.IO;
using System.Xml.Linq;

namespace TLib.Software
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public static class Logger
    {
        private static readonly string logPath = "log.txt";
        public static bool IsEnabled { get; set; } = true;
        /// <summary>
        /// Like "2019-07-31 16:57:50"
        /// </summary>
        private static string Time
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        private static void AppendWithTime(string path,string s)
        {

        }
        public static void WriteLine(object o = null, string path = "")
        {
            if (!IsEnabled)
            {
                return;
            }
            if (path == "")
            {
                path = logPath;
            }
            if (o != null)
            {
                File.AppendAllText(path, $"{Time} {o}{Environment.NewLine}");
            }
            else
            {
                File.AppendAllText(path, );
            }
        }
        public static void WriteException(Exception ex, string message = "", string path = "")
        {
            if (!IsEnabled)
            {
                return;
            }
            if (path == "")
            {
                path = logPath;
            }
            if (ex == null)
            {
                throw new NullReferenceException();
            }
            string s = $"message={ex.Message},targetSite={ex.TargetSite},{ex.StackTrace}";
        }
        public static void Clear(string path = "")
        {
            if (path == "")
            {
                path = logPath;
            }

            File.WriteAllText(path, "");
        }
    }
}
