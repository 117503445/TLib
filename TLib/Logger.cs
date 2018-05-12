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
        public static string File_Log { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "File\\Logger.xml";
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
                return new XDocument(new XElement("Expections"));
            }

        }
        public static void WriteException(Exception e)
        {
            XDocument xml = GetXMLFromDisk();
            xml.Root.Add(new XElement("ex", new XElement("time", DateTime.Now.ToLocalTime().ToString()), new XElement("Message", e.Message), new XElement("Data", e.Data), new XElement("Source", e.Source), new XElement("StackTrace", e.StackTrace), new XElement("TargetSite", e.TargetSite), new XElement("HResult", e.HResult), new XElement("HelpLink", e.HelpLink)));
            SaveXML(xml);
        }
        public static void Write() { }
        private static void SaveXML(XDocument xml) {
            Directory.CreateDirectory(Dir_Log);
            xml.Save(File_Log);
        }
    }
}
