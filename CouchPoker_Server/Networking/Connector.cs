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
        private static int port = 25051;
        private static TcpListener listener = new TcpListener(IPAddress.Any, port);

        public static void StopListener()
        {
            listener.Stop();
        }

        public static Task ConnectClient(UserHandler user)
        {   //event to userhandlera
            TcpClient acceptedClient = null;
            Task t = new Task(() =>
            {
                while (true)
                {
                    Console.WriteLine($"Nasluchiwanie polaczenia na porcie {port}");

                    int id = user._id;

                    listener.Start();

                    try
                    {
                        acceptedClient = listener.AcceptTcpClient();

                    }
                    catch (SocketException socketex)
                    {
                        acceptedClient = null;
                        listener.Stop();
                        continue;
                    }
                    listener.Stop();

                    Console.WriteLine($"polaczenie z {acceptedClient.Client.RemoteEndPoint}");

                    string msg = GetResponseFromRemote(acceptedClient, "SEND_ID");
                    if (msg == null)
                    {
                        acceptedClient.Close();
                        listener.Stop();
                        continue;
                    }

                    bool foundInHistory = false;
                    bool isAlreadyConnected = false;
                    foreach (UserData usr in MainWindow.usersHistory)
                    {
                        if (usr.uID == null || usr.uID.Equals(msg))
                        {
                            foundInHistory = true;

                            foreach (UserHandler u in MainWindow.users)
                            {
                                if (u.UserData.uID.Equals(msg))
                                {
                                    isAlreadyConnected = true;
                                    break;
                                }
                            }
                            if (isAlreadyConnected)
                            {
                                SendToRemote(acceptedClient, $"EXIT");
                                acceptedClient.Close();
                                break;
                            }
                            else
                            {
                                MainWindow.dispatcher.Invoke(() =>
                                {
                                    user.RemoteClient = acceptedClient;
                                    user.UserData = usr;
                                    user.IsReconnecting = true;
                                });
                                SendToRemote(acceptedClient, $"HI_{usr.username}");
                            }
                        }
                    }
                    if (isAlreadyConnected) continue;
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
