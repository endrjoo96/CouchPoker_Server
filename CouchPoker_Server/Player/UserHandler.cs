using CouchPoker_Server.Networking;
using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CouchPoker_Server
{
    public enum STATUS
    {
        CHECK, BET, FOLD, NO_ACTION, NEW_GAME, MY_TURN, ALL_IN, SMALL_BLIND, BIG_BLIND, DEALER
    }

    public class UserHandler
    {
        public delegate void DataReceivedDelegate(DataReceivedEventArgs args);
        public event DataReceivedDelegate DataReceived;

        public bool IsPlaying { get; set; }

        private UserData _userData;
        public UserData UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                UID = value.uID;
                Username = value.username;
                TotalBallance = value.ballance;
            }
        }

        private Controls.User user = null;
        private Receiver receiver;
        private STATUS _status = STATUS.NEW_GAME;
        private TcpClient _tcpClient = null;

        public Card[] cards;

        private bool _isDealer;
        public bool IsDealer {
            get { return _isDealer; }
            set { 
                _isDealer = value;
                user.IsDealer.Visibility = 
                    value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }
        public bool IsReconnecting
        {
            get; set;
        }
        public bool IsActive
        {
            get
            {
                if (_tcpClient == null || Username == null) return false;
                else return true;
            }
        }
        public TcpClient tcpClient
        {
            get { return _tcpClient; }
            set
            {
                _tcpClient = value;
                if (_tcpClient != null)
                {
                    receiver.BeginReceive(_tcpClient);
                }
                else receiver.StopReceiving();

            }
        }

        public string Username
        {
            get { return _userData.username; }
            set
            {
                if (value == null || value == "")
                {
                    user.Username.Content = value;
                    user.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    user.Username.Content = value;
                    user.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public int TotalBallance
        {
            get { return _userData.ballance; }
            set
            {
                user.Total.Content = value.ToString();
                _userData.ballance = value;
            }
        }
        public int CurrentBet
        {
            get { return Int32.Parse(user.Current.Content.ToString()); }
            set { user.Current.Content = value.ToString(); }
        }
        public STATUS Status
        {
            get { return _status; }
            set
            {
                switch (value)
                {
                    case STATUS.FOLD:
                        {
                            ChangeColor(new SolidColorBrush(Colors.Gray));
                            user.Action.Content = value;
                            break;
                        }
                    case STATUS.BET:
                    case STATUS.CHECK:
                        {
                            ChangeColor(new SolidColorBrush(Colors.White));
                            user.Action.Content = value;
                            break;
                        }
                    case STATUS.NO_ACTION:
                    case STATUS.NEW_GAME:
                        {
                            ChangeColor(new SolidColorBrush(Colors.White));
                            user.Action.Content = "";
                            break;
                        }
                    case STATUS.MY_TURN:
                        {
                            ChangeColor(new SolidColorBrush(Colors.White));
                            user.Username.Foreground = new SolidColorBrush(Colors.Yellow);
                            user.Action.Content = "";
                            break;
                        }
                    case STATUS.DEALER:
                        {
                            IsDealer = true;
                            break;
                        }
                }
                _status = value;
            }
        }
        public string UID { get; set; }


        public UserHandler(Controls.User uiControl, UserData data, bool isReconnecting) : this(uiControl, data)
        {
            IsReconnecting = isReconnecting;
        }
        public UserHandler(Controls.User uiControl, UserData data) : this()
        {
            user = uiControl;
            UserData = data;
        }

        public UserHandler()
        {
            IsReconnecting = false;
            receiver = new Receiver();
            receiver.DataReceived += Receiver_DataReceived;
            receiver.ClientDisconnected += Receiver_ClientDisconnected;
        }

        private void Receiver_ClientDisconnected()
        {
            if (!MainWindow.dispatcher.CheckAccess())
                MainWindow.dispatcher.Invoke(new Action(ClearUser));
            else
                ClearUser();
        }

        public DataReceivedEventArgs latestArgs;
        private void Receiver_DataReceived(DataReceivedEventArgs args)
        {
            DataReceived?.Invoke(args);
            if (Status == STATUS.MY_TURN && args.status != STATUS.NO_ACTION)
            {
                InvokeIfRequired(() => {
                    latestArgs = args;
                    Status = args.status;
                });

            }
            Console.WriteLine($"{Username} makes {args.message}");
        }

        private void ChangeColor(SolidColorBrush color)
        {
            user.label1.Foreground = color;
            user.label2.Foreground = color;
            user.label3.Foreground = color;
            user.Current.Foreground = color;
            user.Total.Foreground = color;
            user.Username.Foreground = color;
            user.Action.Foreground = color;
        }

        private void ClearUser()
        {
            if (MainWindow.CountActiveUsers() < 2)
            {
                MainWindow.NotEnoughPlayers();
            }
            if (IsPlaying) Status = STATUS.FOLD;
            while(IsPlaying) { System.Threading.Thread.Sleep(1000); }
            MainWindow.usersHistory.Add(new UserData(_userData));
            UserData = default;
            _tcpClient = null;

        }

        public void SetCards(Card[] cards)
        {
            this.cards = cards;
            user.CARD_1.Source = new BitmapImage(new Uri(CARD.reverse, UriKind.Relative));
            user.CARD_2.Source = new BitmapImage(new Uri(CARD.reverse, UriKind.Relative));
        }

        public void RevealCards()
        {
            user.CARD_1.Source = new BitmapImage(new Uri(cards[0].Path, UriKind.Relative));
            user.CARD_2.Source = new BitmapImage(new Uri(cards[1].Path, UriKind.Relative));
        }

        public static void InvokeIfRequired(Action action)
        {
            if (!MainWindow.dispatcher.CheckAccess())
            {
                MainWindow.dispatcher.Invoke(action);
            }
            else action.Invoke();
        }
    }
}
