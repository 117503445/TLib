using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLib.Net.Udp;
using System.Net.Sockets;
namespace UnitTest
{
    [TestClass]
    public class UdpTest
    {
        [TestMethod]
        public void TUdpClientServerTest()
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
        public void TUdpClientStaticSendTest()
        {
            TUdpServer server = new TUdpServer(900);
            server.UdpReceiveString += (s, e) =>
            {
                var b = e.Equals("Hello");
                Assert.IsTrue(!b);
            };
            TUdpClient.StaticSend("127.0.0.1", 900, "Hello");
            System.Threading.Thread.Sleep(100);
        }
    }
}
