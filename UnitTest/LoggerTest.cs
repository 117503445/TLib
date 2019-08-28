using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLib.Software;

namespace UnitTest
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void WriteExceptionOnceTest()
        {
            string path = "err.json";
            string message = "test exception";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Exception exception = new Exception(message);
            Logger.WriteException(exception);
            Assert.IsTrue(File.Exists(path));
            string s = File.ReadAllText(path);
            Assert.IsTrue(s.Contains(message));
        }
        [TestMethod]
        public void WriteExceptionOnceTwiceTest()
        {
            string path = "err.json";
            string message = "test exception";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Exception exception = new Exception(message);
            Logger.WriteException(exception);
            Logger.WriteException(exception);
            Assert.IsTrue(File.Exists(path));
            string s = File.ReadAllText(path);
            Assert.IsTrue(s.Contains(message));
        }
    }
}
