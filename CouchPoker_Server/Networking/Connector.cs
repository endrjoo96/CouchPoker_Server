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

                    try
                    {
                        listener.Start();
                        acceptedClient = listener.AcceptTcpClient();

                    }
                    catch (SocketException socketex)
                    {
                        Console.WriteLine(socketex.Message);
                        acceptedClient = null;
                        listener.Stop();
                        break;
                    }

                    catch (ObjectDisposedException disposedEx)
                    {
                        Console.WriteLine(disposedEx.Message);
                        listener = new TcpListener(IPAddress.Any, port);
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
                                if (u.IsActive && u.UserData.uID.Equals(msg))
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
                                    user.Status = STATUS.NEW_USER;
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
                            user.UserData = new UserData(uid, msg, MainWindow.startupTokens);
                            user.RemoteClient = acceptedClient;
                            user.IsReconnecting = false;
                            user.Status = STATUS.NEW_USER;
                            MainWindow.usersHistory.Add(user.UserData);
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
