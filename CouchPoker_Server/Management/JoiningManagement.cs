using CouchPoker_Server.Networking;
using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CouchPoker_Server.Management
{
    class JoiningManagement
    {
        private static CancellationTokenSource cts;
        private static List<UserHandler> _users;
        private static List<UserData> _usersHistory;
        private static volatile bool cancelled;

        public static async void Run(List<UserHandler> users, List<UserData> usersHistory)
        {
            _users = users;
            _usersHistory = usersHistory;
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            Task t = Task.Run(() =>
            {
                //token.ThrowIfCancellationRequested();
                int delay = 1000;
                int currentDelay;
                bool serverIsFull = true;
                
                do
                {
                    cancelled = false;
                    foreach (UserHandler user in users)
                    {
                        if (!user.IsActive)
                        {
                            serverIsFull = false;
                            Task x = Connector.ConnectClient(user);
                            while(!x.IsCompleted && !cancelled) { Thread.Sleep(100); }
                            if (cancelled) Connector.StopListener();
                            if (!user.IsActive) _usersHistory.Add(user.UserData);
                            break;
                        }
                        else serverIsFull = true;
                    }
                    currentDelay = serverIsFull ? delay * 10 : delay / 10;
                    Thread.Sleep(currentDelay);
                } while (true);
            }, token);

            try
            {
                await t;
            }
            catch (OperationCanceledException ocex)
            {
                Console.WriteLine(ocex.Message);
            }
            finally
            {
                t.Dispose();
            }

        }

        public static void Refresh()
        {
            cancelled = true;
            //cts.Cancel();

        }


    }
}
