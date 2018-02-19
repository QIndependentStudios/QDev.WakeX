using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QDev.WakeX.Core
{
    public class XboxWakeService
    {
        private const string PingPacket = "DD00000A000000000000000400000002";
        private const int Port = 5050;
        private const int WakePacketBurstAmount = 10;
        private const int WakePacketCooldown = 500;
        private const int PingResponseCheckAmount = 5;
        private const int PingResponseCheckCooldown = 1000;
        private const int AttemptAmount = 3;
        private const int AttemptCooldown = 1000;

        private static byte[] GetWakePacket(string xboxLiveID)
        {
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
            return packet.ToArray();
        }

        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public async Task<bool> WakeAsync(string ipAddress = "10.0.0.9", string xboxLiveID = "FD00A8E459035C35")
        {
            using (var client = new UdpClient(AddressFamily.InterNetwork))
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), Port);
                client.Connect(endPoint);

                var packet = GetWakePacket(xboxLiveID);
                var pingPacket = StringToByteArray(PingPacket);

                for (int i = 0; i < AttemptAmount; i++)
                {
                    Console.WriteLine($"Attempt #{i + 1} starting...");
                    for (int j = 0; j < WakePacketBurstAmount; j++)
                    {

                        Console.WriteLine($"Sending #{j + 1} of {WakePacketBurstAmount} wake packets in burst...");
                        await client.SendAsync(packet, packet.Length);
                        await Task.Delay(WakePacketCooldown);
                    }

                    Console.WriteLine($"Ping for attempt #{i + 1}...");
                    await client.SendAsync(pingPacket, pingPacket.Length);
                    for (int j = 0; j < PingResponseCheckAmount; j++)
                    {
                        await Task.Delay(PingResponseCheckCooldown);
                        Console.WriteLine($"Checking for ping response #{j + 1}.");
                        if (client.Available > 0)
                        {
                            byte[] data = client.Receive(ref endPoint);
                            Console.WriteLine("Response received. Xbox is now on.");
                            return true;
                        }
                    }
                    await Task.Delay(AttemptCooldown);
                }

                return false;
            }
        }
    }
}
