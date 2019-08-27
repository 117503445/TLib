using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLib.Net.Udp;

namespace UnitTest
{
    [TestClass]
    public class UdpTest
    {
        [TestMethod]
        public void UdpClientServerTest()
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
    }
}
