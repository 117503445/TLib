using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TLib;
using TLib.Net.Udp;
namespace UnitTest
{
    [TestClass]
    public class CSharpTest
    {

        [TestMethod]
        public void RandomReservedTest()
        {
            var listA = new List<int>();
            var listB = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                listA.Add(i);
                listB.Add(i);
            }
            CsharpHelper.RandomReserve(ref listA);
            bool equal = true;
            for (int i = 0; i < listA.Count; i++)
            {
                if (listA[i] != listB[i])
                {
                    equal = false;
                    break;
                }
            }
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void UdpTest()
        {
            TUdpServer server = new TUdpServer();
            server.UdpReceiveString += (s, e) =>
            {
                var b = e.Equals("Hello");
                Assert.IsTrue(!b);
            };
            var client = new TUdpClient();
            client.Send("127.0.0.1", 800, "Hello");
            System.Threading.Thread.Sleep(100);
        }

        [TestMethod]
        public void SwapTest()
        {
            int a = 2;
            int b = 3;
            CsharpHelper.Swap(ref a, ref b);
            Assert.AreEqual(a, 3);
            Assert.AreEqual(b, 2);
        }
    }
}
