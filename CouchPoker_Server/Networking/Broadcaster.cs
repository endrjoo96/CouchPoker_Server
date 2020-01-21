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
        private const int port = 25051;
        public Broadcaster()
        {
            new Task(() =>
            {

                var addressInt = BitConverter.ToInt32(GetIPAddress().GetAddressBytes(), 0);
                var maskInt = BitConverter.ToInt32(GetSubnetMask(GetIPAddress()).GetAddressBytes(), 0);
                var broadcastInt = addressInt | ~maskInt;
                var broadcast = new IPAddress(BitConverter.GetBytes(broadcastInt));

                IPEndPoint broadcastAddress = new IPEndPoint(broadcast, port);
                UdpClient udpClient = new UdpClient();
                string broadcastMessage = "CouchPoker_Server|" + MainWindow.servername;
                byte[] buffer = Encoding.UTF8.GetBytes(broadcastMessage);
                bool flip = false;
                while (true)
                {
                    while (!isServerFull)
                    {
                        if (flip)
                        {
                            udpClient.Send(buffer, buffer.Length, broadcastAddress);
                        }
                        else
                        {
                            udpClient.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, port));
                        }
                        flip = !flip;
                        System.Threading.Thread.Sleep(500);
                    }

                    System.Threading.Thread.Sleep(2000);
                }
            }).Start();
        }

        public void PauseBroadcasting()
        {
            isServerFull = true;
        }

        public void ResumeBroadcasting()
        {
            isServerFull = false;
        }

        public IPAddress GetIPAddress()
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
