﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TLib.Net.Udp
{
    /// <summary>
    /// 基于UDP的服务器
    /// </summary>
    public class TUdpServer
    {
        public TUdpServer(int sort = 800, bool running = true)
        {
            Running = running;
            Sort = sort;
            if (running)
            {
                Start();
            }
        }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool Running { get; set; } = false;
        /// <summary>
        /// 端口
        /// </summary>
        public int Sort { get; set; } = 800;
        private UdpClient Udp { get; set; }

        public event EventHandler<string> UdpReceiveString;
        public event EventHandler<UdpReceiveResult> UdpReceived;
        public event EventHandler<byte[]> UdpReceiveBytes;

        public void Start()
        {
            try
            {
                Udp = new UdpClient(Sort);
            }
            catch (Exception)
            {

                throw;
            }
            Running = true;
            Task.Run(async () =>
            {
                while (true)
                {
                    var result = await Udp.ReceiveAsync().ConfigureAwait(false);
                    UdpReceived?.Invoke(this, result);
                    UdpReceiveBytes?.Invoke(this, result.Buffer);
                    UdpReceiveString?.Invoke(this, Encoding.Default.GetString(result.Buffer));
                }
            });
        }
        public void Stop()
        {
            Running = false;
            Udp.Dispose();
            Udp = null;
        }
    }
}
