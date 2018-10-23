using System;
using System.IO;
using System.Xml.Linq;

namespace TLib.Software
{
    /// <summary>
    /// 通用的Logger
    /// </summary>
    public static class Logger
    {
        public static string Dir_Log { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "File\\";
        private static string File_Log { get { return Dir_Log + "Logger.xml"; } }
        /// <summary>
        /// 读取Log
        /// </summary>
        /// <returns></returns>
        private static XDocument GetXMLFromDisk()
        {
            try
            {
                return XDocument.Load(File_Log);
            }
            catch (Exception)
            {
                if (File.Exists(File_Log))
                {
                    File.Move(File_Log, File_Log + ".damage" + new Random().Next());
                }
                return new XDocument(new XComment("TLib的Logger,Ver_1.0"), new XElement("Logs"));
            }

        }
        /// <summary>
        /// 写入异常
        /// </summary>
        /// <param name="e"></param>
        /// <param name="handled"></param>
        /// <param name="info"></param>
        public static void WriteException(Exception e, bool handled = true, string info = "")
        {
            XDocument xml = GetXMLFromDisk();
            xml.Root.Add(new XElement("Expection", new XElement("Time", DateTime.Now.ToLocalTime().ToString()), new XElement("Message", e.Message), new XElement("Data", e.Data), new XElement("Source", e.Source), new XElement("StackTrace", e.StackTrace), new XElement("TargetSite", e.TargetSite), new XElement("HResult", e.HResult), new XElement("HelpLink", e.HelpLink), new XAttribute("Handled", handled), new XAttribute("Info", info)));
            SaveXMLToDisk(xml);
        }
        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="info"></param>
        /// <param name="type">Log的类型</param>
        public static void Write(object info, string type = "info")
        {
            XDocument xml = GetXMLFromDisk();
            xml.Root.Add(new XElement("Log", new XAttribute("Time", DateTime.Now.ToLocalTime().ToString()), new XAttribute("Info", info.ToString()), new XAttribute("Type", type)));
            SaveXMLToDisk(xml);
        }
        /// <summary>
        /// 保存Log
        /// </summary>
        /// <param name="xml"></param>
        private static void SaveXMLToDisk(XDocument xml)
        {
            Directory.CreateDirectory(Dir_Log);
            xml.Save(File_Log);
        }
    }
}
