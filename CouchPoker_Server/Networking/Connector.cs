using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CouchPoker_Server.Networking
{
    class Connector
    {
        public static Task ConnectClient(UserHandler user)
        {   //event to userhandlera
            TcpClient acceptedClient = null;
            Task t = new Task(() =>
            {
                while (true)
                {
                    int port = 25051;
                    Console.WriteLine($"Nasluchiwanie polaczenia na porcie {port}");


                    TcpListener listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();
                    acceptedClient = listener.AcceptTcpClient();
                    listener.Stop();

                    Console.WriteLine($"polaczenie z {acceptedClient.Client.RemoteEndPoint}");

                    string msg = GetResponseFromRemote(acceptedClient, "send me ur uid");
                    if (msg == null)
                    {
                        listener.Stop();
                        continue;
                    }

                    bool foundInHistory = false;
                    foreach (UserData usr in MainWindow.usersHistory)
                    {
                        if (usr.uID.Equals(msg))
                        {
                            foundInHistory = true;
                            MainWindow.dispatcher.Invoke(() =>
                            {
                                user.tcpClient = acceptedClient;
                                user.userData = usr;
                            });
                        }
                    }
                    if (!foundInHistory)
                    {
                        string uid = msg;
                        msg = GetResponseFromRemote(acceptedClient, "ur nickname");
                        MainWindow.dispatcher.Invoke(() =>
                        {
                            user.userData = new UserData(uid, msg, 10000);
                            user.tcpClient = acceptedClient;
                        });
                    }
                    break;
                }









                /*while (true)
                {
                    byte[] buffer = new byte[acceptedClient.ReceiveBufferSize];
                    int data = ns.Read(buffer, 0, acceptedClient.ReceiveBufferSize);

                    if (data == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, data);
                    Console.WriteLine("wiadomosc: " + message);
                }
                Console.WriteLine($"Klient {acceptedClient.Client.RemoteEndPoint} rozlaczyl sie, nasluchuje nowego klienta...");
                acceptedClient = null;
                ns.Dispose();
                listener.Stop();*/

            });
            t.Start();
            return t;
        }

        private static string GetResponseFromRemote(TcpClient client, string question)
        {
            byte[] message = Encoding.UTF8.GetBytes(question);
            client.GetStream().Write(message, 0, message.Length);

            byte[] buffer = new byte[client.ReceiveBufferSize];
            int data = client.GetStream().Read(buffer, 0, client.ReceiveBufferSize);

            if (data == 0) return null;

            string msg = Encoding.UTF8.GetString(buffer, 0, data);
            return msg;
        }
    }
}
