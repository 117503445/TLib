﻿using Newtonsoft.Json;
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
        /// <summary>
        /// 默认的Log路径 Log.txt
        /// </summary>
        public static string LogPath = "log.txt";
        /// <summary>
        /// 默认的错误Log路径 err.json
        /// </summary>
        public static string ErrPath = "err.json";
        /// <summary>
        /// 返回当前时间,Like "2019-07-31 16:57:50"的字符串
        /// </summary>
        private static string Time
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 给字符串增加时间和换行
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string AddTimeAndNewLine(string s)
        {
            return $"{Time}\t{s}{Environment.NewLine}";
        }
        /// <summary>
        /// 给字符串增加时间和换行后追加到指定路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="s"></param>
        private static void AppendWithTimeAndNewLine(string path, string s)
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(new FileInfo(path).DirectoryName);
            }
            File.AppendAllText(path, AddTimeAndNewLine(s));
        }
        /// <summary>
        /// 输出普通的日志到 LogPath(默认),自动增加时间和换行
        /// </summary>
        /// <param name="o"></param>
        /// <param name="path"></param>
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

            AppendWithTimeAndNewLine(path, s);
        }
        /// <summary>
        /// Json序列化 exception 到 ErrPath(默认),自动增加时间和换行
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="userMessage"></param>
        /// <param name="path"></param>
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

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(new FileInfo(path).DirectoryName);
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
    /// <summary>
    /// 给 Exception 带上 时间和用户的注释
    /// </summary>
    internal class LoggerException
    {
        public string Time;
        public string UserMessage;
        public Exception innerException;
        /// <summary>
        /// 给 Exception 带上 时间和用户的注释
        /// </summary>
        public LoggerException(string message, Exception innerException)
        {
            UserMessage = message;
            this.innerException = innerException;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
