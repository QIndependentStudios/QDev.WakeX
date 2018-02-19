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
        private const string PingPacket = "DD00000A000000000000000400000002";
        private const int Port = 5050;
        private const int RetryDelay = 100;

        public void Wake(string ipAddress = "10.0.0.9", string xboxLiveID = "FD00A8E459035C35")
        {
            for (int i = 0; i < 200; i++)
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Blocking = false;
                    socket.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), Port));


                    var payload = new List<byte>
                    {
                        0x00,
                        BitConverter.GetBytes((char)xboxLiveID.Length)[0]
                    };
                    payload.AddRange(Encoding.UTF8.GetBytes(xboxLiveID));
                    payload.Add(0x00);

                    var header = new List<byte>
                    {
                        0xdd,
                        0x02,
                        0x00,
                        BitConverter.GetBytes((char)payload.Count)[0],
                        0x00,
                        0x00
                    };

                    var packet = header.Concat(payload).ToList();
                    Console.WriteLine($"Attempt {i}. Sending {BitConverter.ToString(packet.ToArray())}");
                    socket.Send(packet.ToArray());
                    Thread.Sleep(RetryDelay);
                }
            }
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
