using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TLib.Software
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public static class Logger
    {
        public static string LogPath = "log.txt";
        public static string ErrPath = "err.json";
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
        /// <summary>
        /// 增加时间和换行
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string AddTimeAndNewLine(string s)
        {
            return $"{Time}\t{s}{Environment.NewLine}";
        }
        private static void AppendWithTime(string path, string s)
        {
            File.AppendAllText(path, AddTimeAndNewLine(s));
        }
        public static void WriteLine(object o = null, string path = "")
        {
            if (path == "")
            {
                path = LogPath;
            }

            string s = "";
            if (o != null)
            {
                s = o.ToString();
            }

            AppendWithTime(path, s);
        }
        public static void WriteException(Exception ex, string userMessage = "", string path = "")
        {

            if (path == "")
            {
                path = ErrPath;
            }
            if (ex == null)
            {
                throw new NullReferenceException();
            }

            LoggerException exception = new LoggerException(userMessage, ex);

            List<LoggerException> exs;
            try
            {
                string json = File.ReadAllText(path);
                exs = JsonConvert.DeserializeObject<List<LoggerException>>(json);
                exs.Add(exception);
            }
            catch (Exception)
            {
                exs = new List<LoggerException>
                {
                    exception
                };
            }
            File.WriteAllText(path, JsonConvert.SerializeObject(exs,Formatting.Indented));
        }
        /// <summary>
        /// 默认清空 logPath
        /// </summary>
        /// <param name="path"></param>
        public static void Clear(string path = "")
        {
            if (path == "")
            {
                path = LogPath;
            }

            File.WriteAllText(path, "");
        }
    }
    internal class LoggerException
    {
        public string Time;
        public string UserMessage;
        public Exception innerException;
        public LoggerException(string message, Exception innerException)
        {
            UserMessage = message;
            this.innerException = innerException;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
