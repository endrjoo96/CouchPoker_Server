using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Networking
{
    public class Broadcaster
    {
        public bool isServerFull = false;
        public Broadcaster()
        {

            /* 
             
static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
{
    using (UdpClient c = new UdpClient(srcPort))
        c.Send(data, data.Length, dstIp, dstPort);
}

             */



            new Task(() =>
            {

                var addressInt = BitConverter.ToInt32(GetIPAddress().GetAddressBytes(), 0);
                var maskInt = BitConverter.ToInt32(GetSubnetMask(GetIPAddress()).GetAddressBytes(), 0);
                var broadcastInt = addressInt | ~maskInt;
                var broadcast = new IPAddress(BitConverter.GetBytes(broadcastInt));

                IPEndPoint broadcastAddress = new IPEndPoint(broadcast, 25051);
                UdpClient udpClient = new UdpClient();
                string broadcastMessage = "CouchPoker_Server|" + "stationartServer1";
                byte[] buffer = Encoding.UTF8.GetBytes(broadcastMessage);
                while (true)
                {
                    while (!isServerFull)
                    {
                        udpClient.Send(buffer, buffer.Length, broadcastAddress);
                        Console.WriteLine($"sent udp packet to {broadcast.ToString()}");

                        System.Threading.Thread.Sleep(1500);
                    }

                    System.Threading.Thread.Sleep(10000);
                }
            }).Start();
            /*
             
var addressInt = BitConverter.ToInt32(address.GetAddressBytes(), 0);
var maskInt = BitConverter.ToInt32(mask.GetAddressBytes(), 0);
var broadcastInt = addressInt | ~maskInt;
var broadcast = new IPAddress(BitConverter.GetBytes(broadcastInt));

             */
        }

        public void PauseBroadcasting()
        {
            isServerFull = true;
        }

        public void ResumeBroadcasting()
        {
            isServerFull = false;
        }

        private IPAddress GetIPAddress()
        {
            IPAddress localAddress;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localAddress = endPoint.Address;
            }
            return localAddress;
        }

        private IPAddress GetSubnetMask(IPAddress address)
        {



            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            UnicastIPAddressInformation info = null;
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    !(Interface.OperationalStatus == OperationalStatus.Up)) continue;
                Console.WriteLine(Interface.Description);
                UnicastIPAddressInformationCollection UnicastIPInfoCol = Interface.GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicatIPInfo in UnicastIPInfoCol)
                {
                    if (UnicatIPInfo.Address.ToString() == address.ToString())
                    {
                        Console.WriteLine("\tIP Address is {0}", UnicatIPInfo.Address);
                        Console.WriteLine("\tSubnet Mask is {0}", UnicatIPInfo.IPv4Mask);
                        info = UnicatIPInfo;
                    }
                }
            }


            return info.IPv4Mask;
        }
    }
}
