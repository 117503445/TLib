using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class TIOTest
    {
        [TestMethod]
        public void CopyFolderTest()
        {
            Directory.CreateDirectory("source");
            File.WriteAllText("source/1.txt", "");
            Directory.CreateDirectory("source/child");
            File.WriteAllText("source/child/1.txt", "");

            TLib.IO.TIO.CopyFolder("source", "dest");

            Assert.IsTrue(Directory.Exists("dest"));
            Assert.IsTrue(Directory.Exists("dest/child"));
            Assert.IsTrue(File.Exists("dest/1.txt"));
            Assert.IsTrue(File.Exists("dest/child/1.txt"));

            Directory.Delete("source", true);
            Directory.Delete("dest", true);
        }
    }
}
