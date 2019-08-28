using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TLib.Net.Udp
{
    public class TUdpClient
    {
        public int LocalPort { get; set; } = 801;

        public TUdpClient(int localPort = 801)
        {
            LocalPort = localPort;
        }

        private UdpClient Udp { get; set; }
        public void Send(string hostname, int port, string message)
        {
            Send(LocalPort, hostname, port, Encoding.Default.GetBytes(message));
        }
        public void Send(int localPort, string hostname, int port, string message)
        {
            Send(localPort, hostname, port, Encoding.Default.GetBytes(message));
        }
        public void Send(string hostname, int port, byte[] message)
        {
            Send(LocalPort, hostname, port, message);
        }
        public void Send(int localPort, string hostname, int port, byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (Udp == null)
            {
                Udp = new UdpClient(localPort);
            }
            Udp.Send(message, message.Length, hostname, port);
            Udp.Dispose();
            Udp = null;
        }



        public static void StaticSend(string hostname, int port, string message)
        {
            StaticSend(hostname, port, Encoding.Default.GetBytes(message));
        }
        public static void StaticSend(string hostname, int port, byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            using (UdpClient udp = new UdpClient())
            {
                udp.Send(message, message.Length, hostname, port);
            }
        }
    }
}
