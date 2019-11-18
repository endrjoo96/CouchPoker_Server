using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Player
{
    public class DealerHandler
    {
        private Controls.Dealer dealer;
        private int _pot;
        public int Pot { 
            get
            {
                return _pot;
            }
            set
            {
                _pot = value;
                dealer.Pot.Content = value;
            }
        }

        public DealerHandler(Controls.Dealer dealer)
        {
            this.dealer = dealer;
        }
    }
}
