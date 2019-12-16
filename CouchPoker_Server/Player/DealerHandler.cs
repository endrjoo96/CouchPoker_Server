using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Player
{
    public class DealerHandler
    {
        public enum DEALER_STATE
        {
            SHOW_POT, SHOW_INFO
        }
        private Controls.Dealer dealer;
        private int _pot;
        public int Pot {
            get {
                return _pot;
            }
            set {
                _pot = value;
                dealer.Pot.Content = value;
            }
        }

        public string Message {
            set {
                if (State != DEALER_STATE.SHOW_INFO)
                    State = DEALER_STATE.SHOW_INFO;
                dealer.InfoLabel.Text = value;
            }
        }

        private DEALER_STATE _state = DEALER_STATE.SHOW_POT;
        public DEALER_STATE State {
            private get { return _state; }
            set {
                _state = value;
                switch (value)
                {
                    case DEALER_STATE.SHOW_INFO:
                    {
                        dealer.Dollar.Visibility = System.Windows.Visibility.Hidden;
                        dealer.Pot.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    }
                    case DEALER_STATE.SHOW_POT:
                    {
                        dealer.Dollar.Visibility = System.Windows.Visibility.Visible;
                        dealer.Pot.Visibility = System.Windows.Visibility.Visible;
                        dealer.InfoLabel.Text = "Pot:";
                        break;
                    }
                }
            }
        }

        public DealerHandler(Controls.Dealer dealer)
        {
            this.dealer = dealer;
        }
    }
}
