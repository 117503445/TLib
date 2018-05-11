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
        public static string File_Log { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "File/Logger.xml";
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
                return new XDocument();
            }

        }
        public static void WriteException(Exception e)
        {
            XDocument xml = GetXMLFromDisk();
            xml.Root.Add(new XElement(

                "ex"));
            xml.Save(File_Log);
        }
        public static void Write() { }
    }
}
