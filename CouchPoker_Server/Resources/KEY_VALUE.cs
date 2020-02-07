using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Resources
{
    public static class KEY_VALUE  // "|" sign is used in commands that sends specific values to client.
    {
        public const string FOLD = "FOLD";
        public const string CHECK = "CHECK";
        public const string RAISE = "RAISE";
        public const string STARTED_NEW_ROUND = "STARTED_NEW_ROUND";
        public const string YOUR_BALANCE = "YOUR_BALANCE|";
        public const string YOUR_BET = "YOUR_BET|";
        public const string CHECK_VALUE = "CHECK_VALUE|";
        public const string BIG_BLIND = "BIG_BLIND|";
        public const string END_OF_TRANSMISSION = "EOT";
        public const string YOUR_FIGURE = "YOUR_FIGURE|";
        public const string WAITING_FOR_PLAYER = "WAITING_FOR_YOUR_MOVE";
        public const string DISCONNECTING_SIGNAL = "DISCONNECT";

        public const string BROADCAST_MESSAGE = "CouchPoker_Server|";
    }
}
