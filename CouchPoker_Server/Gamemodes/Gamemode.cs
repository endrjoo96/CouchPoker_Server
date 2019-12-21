using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Gamemodes
{
    public abstract class Gamemode
    {
        public abstract Task RunGame();
    }
}
