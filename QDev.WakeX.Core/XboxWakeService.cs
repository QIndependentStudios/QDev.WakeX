using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QDev.WakeX.Core
{
    public class XboxWakeService
    {
        public void Wake()
        {
            var xboxLiveID = "FD00A8E459035C35";
            using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Blocking = false;
                //socket.Bind(new IPEndPoint(IPAddress.Parse(""), 0));
                socket.Connect(new IPEndPoint(IPAddress.Parse("10.0.0.9"), 5050));


                var payload = new List<byte>();
                payload.Add(0x00);
                payload.AddRange(Encoding.UTF8.GetBytes(xboxLiveID.Length.ToString()));
                payload.AddRange(Encoding.UTF8.GetBytes(xboxLiveID));
                payload.Add(0x00);

                var header = new List<byte>();
                header.Add(0xdd);
                header.Add(0x02);
                header.Add(0x00);
                header.AddRange(Encoding.UTF8.GetBytes(payload.Count.ToString()));
                header.Add(0x00);
                header.Add(0x00);

                var packet = header.Concat(payload).ToList();

                for (int i = 0; i < 10; i++)
                {
                    socket.Send(packet.ToArray());
                    Thread.Sleep(new TimeSpan(0, 0, 1));
                }
            }
        }
    }
}
