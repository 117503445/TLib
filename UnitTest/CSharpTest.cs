using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class CSharpTest
    {

        [TestMethod]
        public void UdpTest()
        {
            TLib.Net.Udp.TUdpServer server = new TLib.Net.Udp.TUdpServer();
            server.UDP_ReceiveString += (s, e) =>
            {
                
                System.Console.WriteLine(e);
                if (e == "Hello")
                {
                    Assert.Fail();
                }
            };
            var client = new TLib.Net.Udp.TUdpClient();
            client.Send("127.0.0.1", 800, "Hello");
            System.Threading.Thread.Sleep(100);
        }

        [TestMethod]
        public void SwapTest()
        {
            int a = 2;
            int b = 3;
            TLib.CSharp.Swap(ref a, ref b);
            if (a != 3 || b != 2)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void RandomReserveTest()
        {
            List<int> list = new List<int> { 5, 3, 6, 4, 76, 43 };
            List<int> list2 = new List<int> { 5, 3, 6, 4, 76, 43 };
            TLib.CSharp.RandomReserve(ref list);
            if (list.Equals(list2))
            {
                Assert.Fail();
            }
        }
    }
}
