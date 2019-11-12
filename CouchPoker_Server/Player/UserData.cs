using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Player
{
    public struct UserData
    {
        public string uID;
        public string username;
        public int ballance;

        public UserData(string _uid, string _username, int _ballance)
        {
            uID = _uid;
            username = _username;
            ballance = _ballance;
        }

        public UserData(UserData data) : this(data.uID, data.username, data.ballance) { }

        
    }
}
