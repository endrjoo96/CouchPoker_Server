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
        public static void Run(List<UserHandler> users, List<UserData> usersHistory)
        {
            Task t = new Task(() =>
            {
                int delay = 1000;
                int currentDelay;
                bool serverIsFull = true;
                do
                {
                    foreach (UserHandler user in users)
                    {
                        if (!user.IsActive)
                        {
                            serverIsFull = false;
                            Connector.ConnectClient(user).Wait();
                            break;
                        }
                        else serverIsFull = true;
                    }
                    currentDelay = serverIsFull ? delay * 10 : delay/10;
                    Thread.Sleep(currentDelay);
                } while (true);

            });
            t.Start();
        }
    }
}
