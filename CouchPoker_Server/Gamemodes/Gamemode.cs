using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Gamemodes
{
    public abstract class Gamemode
    {
        protected int BiddValue = 20;
        protected int CardsForUsers = 2;
        public abstract void RunGame();
    }
}
