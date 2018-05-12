using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLib
{
    public static class Logger
    {
        public static string Dir_Log { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "File\\";
        private static string File_Log { get { return Dir_Log + "Logger.xml"; } }
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
        public static void WriteException(Exception e, bool handled = true, string info = "")
        {
            XDocument xml = GetXMLFromDisk();
            xml.Root.Add(new XElement("Expection", new XElement("Time", DateTime.Now.ToLocalTime().ToString()), new XElement("Message", e.Message), new XElement("Data", e.Data), new XElement("Source", e.Source), new XElement("StackTrace", e.StackTrace), new XElement("TargetSite", e.TargetSite), new XElement("HResult", e.HResult), new XElement("HelpLink", e.HelpLink), new XAttribute("Handled", handled), new XAttribute("Info", info)));
            SaveXML(xml);
        }
        public static void Write(string info, string type = "info")
        {
            XDocument xml = GetXMLFromDisk();
            xml.Root.Add(new XElement("Log", new XAttribute("Time", DateTime.Now.ToLocalTime().ToString()), new XAttribute("Info", info), new XAttribute("Type", type)));
            SaveXML(xml);
        }
        private static void SaveXML(XDocument xml)
        {
            Directory.CreateDirectory(Dir_Log);
            xml.Save(File_Log);
        }
    }
}
