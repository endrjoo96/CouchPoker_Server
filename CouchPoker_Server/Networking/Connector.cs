using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static CouchPoker_Server.Misc.Networking;

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

                    string msg = GetResponseFromRemote(acceptedClient, "SEND_ID");
                    if (msg == null)
                    {
                        listener.Stop();
                        continue;
                    }

                    bool foundInHistory = false;
                    foreach (UserData usr in MainWindow.usersHistory)
                    {
                        if (usr.uID == null || usr.uID.Equals(msg))
                        {
                            foundInHistory = true;
                            MainWindow.dispatcher.Invoke(() =>
                            {
                                user.RemoteClient = acceptedClient;
                                user.UserData = usr;
                                user.IsReconnecting = true;
                            });
                        }
                    }
                    if (!foundInHistory)
                    {
                        string uid = msg;
                        msg = GetResponseFromRemote(acceptedClient, "SEND_NICKNAME");
                        MainWindow.dispatcher.Invoke(() =>
                        {
                            user.UserData = new UserData(uid, msg, 10000);
                            user.RemoteClient = acceptedClient;
                            user.IsReconnecting = false;
                        });
                    }
                    break;
                }
            });
            t.Start();
            return t;
        }

        
    }
}
