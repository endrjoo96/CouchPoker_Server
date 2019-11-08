using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Networking
{
    class Worker
    {
        public static void Run()
        {   //event to userhandlera
            Task t = new Task(() =>
            {
                while (true)
                {
                    int port = 25051;
                    Console.WriteLine($"Nasluchiwanie polaczenia na porcie {port}");
                    TcpListener listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine($"polaczenie z {client.Client.RemoteEndPoint}");
                    NetworkStream ns = client.GetStream();
                    while (client.Connected)
                    {
                        byte[] buffer = new byte[client.ReceiveBufferSize];
                        int data = ns.Read(buffer, 0, client.ReceiveBufferSize);

                        if (data == 0) break;

                        string message = Encoding.UTF8.GetString(buffer, 0, data);
                        Console.WriteLine("wiadomosc: " + message);
                    }
                    Console.WriteLine($"Klient {client.Client.RemoteEndPoint} rozlaczyl sie, nasluchuje nowego klienta...");
                    client = null;
                    ns.Dispose();
                    listener.Stop();
                }
            });
            t.Start();
        }
    }
}
