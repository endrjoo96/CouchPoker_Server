﻿using CouchPoker_Server.Management;
using CouchPoker_Server.Networking;
using CouchPoker_Server.Player;
using CouchPoker_Server.Resources;
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
        CHECK, RAISE, FOLD, NO_ACTION, NEW_GAME, MY_TURN, ALL_IN, SMALL_BLIND, BIG_BLIND, DEALER, WINNER, NEW_USER, BANKRUPT
    }

    public class UserHandler
    {
        public delegate void DataReceivedDelegate(DataReceivedEventArgs args);
        public event DataReceivedDelegate DataReceived;

        public int _id;

        private CardsOnHand _cardsOnHand;
        public bool IsPlaying { get; set; }

        private UserData _userData;
        public UserData UserData {
            get { return _userData; }
            set {
                _userData = value;
                UID = value.uID;
                Username = value.username;
                TotalBallance = value.ballance;
            }
        }

        private Controls.User user = null;
        private Receiver receiver;
        private STATUS _status = STATUS.NEW_GAME;
        private TcpClient _remoteClient = null;
        private Set _currentSet;

        public Card[] cards;
        public Set CurrentSet {
            get {
                return _currentSet;
            }
            set {
                _currentSet = value;
                Send_CurrentFigure();
            }
        }

        private bool _isDealer;
        public bool IsDealer {
            get { return _isDealer; }
            set {
                _isDealer = value;
                user.IsDealer.Visibility =
                    value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }
        public bool IsReconnecting {
            get; set;
        }
        public bool IsActive {
            get {
                if (_remoteClient == null || Username == null) return false;
                else return true;
            }
        }
        public TcpClient RemoteClient {
            get { return _remoteClient; }
            set {
                _remoteClient = value;
                if (_remoteClient != null)
                {
                    if (!receiver.IsRunning)
                        receiver.BeginReceive(_remoteClient);
                }
                else receiver.StopReceiving();

            }
        }
        public string Username {
            get { return _userData.username; }
            set {
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
        public int TotalBallance {
            get { return _userData.ballance; }
            set {
                user.Total.Content = value.ToString();
                _userData.ballance = value;
                if (value == 0) Status = STATUS.BANKRUPT;
            }
        }
        public int CurrentBet {
            get { return Int32.Parse(user.Current.Content.ToString()); }
            set {
                user.Current.Content = value.ToString();
                if (value == 0)
                {
                    user.Current.Visibility = System.Windows.Visibility.Hidden;
                }
                else user.Current.Visibility = System.Windows.Visibility.Visible;
                user.label3.Visibility = user.Current.Visibility;
            }
        }
        public STATUS Status {
            get { return _status; }
            set {
                switch (value)
                {
                    case STATUS.BANKRUPT:
                    {
                        Status = STATUS.FOLD;
                        user.Action.Content = value;
                        break;
                    }
                    case STATUS.FOLD:
                    {
                        ChangeColor(new SolidColorBrush(Colors.Gray));
                        user.Action.Content = value;
                        SetCardsVisibility(false);
                        IsPlaying = false;
                        CurrentBet = 0;
                        break;
                    }
                    case STATUS.ALL_IN:
                    case STATUS.RAISE:
                    case STATUS.CHECK:
                    {
                        ChangeColor(new SolidColorBrush(Colors.White));
                        user.Action.Content = value;
                        IsPlaying = true;
                        SetCardsVisibility(true);
                        break;
                    }
                    case STATUS.NO_ACTION:
                    {
                        Send_GameInfo(0, 0);
                        ChangeColor(new SolidColorBrush(Colors.White));
                        user.Action.Content = "";
                        break;
                    }
                    case STATUS.NEW_GAME:
                    {
                        ChangeColor(new SolidColorBrush(Colors.White));
                        user.Action.Content = "";
                        SetCardsVisibility(true);
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
                    case STATUS.WINNER:
                    {
                        ChangeColor(new SolidColorBrush(Colors.Yellow));
                        user.Action.Content = "YOU WIN!";
                        break;
                    }
                    case STATUS.NEW_USER:
                    {
                        ChangeColor(new SolidColorBrush(Colors.Gray));
                        user.Action.Content = "";
                        SetCardsVisibility(false);
                        IsPlaying = false;
                        CurrentBet = 0;
                        break;
                    }
                }
                _status = value;
            }
        }
        public string UID { get; set; }
        
        public UserHandler(Controls.User uiControl, UserData data, int cardsCount, bool isReconnecting) : this(uiControl, data, cardsCount)
        {
            IsReconnecting = isReconnecting;
        }
        public UserHandler(Controls.User uiControl, UserData data, int cardsCount) : this()
        {
            user = uiControl;
            _cardsOnHand = new CardsOnHand(ref user.cardsOnHand, cardsCount);
            UserData = data;

            Status = STATUS.FOLD;
            user.Action.Content = "";

            CurrentBet = 0;
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
            Misc.Threading.InvokeIfRequired(() =>
            {
                ClearUser();
            });
        }

        public DataReceivedEventArgs latestArgs;
        private void Receiver_DataReceived(DataReceivedEventArgs args)
        {
            DataReceived?.Invoke(args);
            if (Status == STATUS.MY_TURN && args.status != STATUS.NO_ACTION)
            {
                Misc.Threading.InvokeIfRequired(() =>
                {
                    latestArgs = args;
                    Status = args.status;
                });

            }
        }

        private void ChangeColor(SolidColorBrush color)
        {
            user.label1.Foreground = color;
            user.label3.Foreground = color;
            user.Current.Foreground = color;
            user.Total.Foreground = color;
            user.Username.Foreground = color;
            user.Action.Foreground = color;
        }

        private void ClearUser()
        {
            if (IsActive)
            {
                if (MainWindow.CountPlayingUsers() <= 2)
                {
                    MainWindow.NotEnoughPlayers();
                }
                if (IsPlaying)
                {
                    Status = STATUS.FOLD;
                    IsPlaying = false;
                }
                for(int i=0;i< MainWindow.usersHistory.Count;i++)
                {
                    if (MainWindow.usersHistory[i].uID == UserData.uID)
                        MainWindow.usersHistory[i] = new UserData(UserData);
                }
                UserData = default;
                if (RemoteClient != null)
                {
                    RemoteClient.Close();
                    RemoteClient.Dispose();
                    RemoteClient = null;
                }
                JoiningManagement.Refresh();
            }
        }
        
        public void SetCards(Card[] cards)
        {
            this.cards = cards;
            if (_cardsOnHand == null)
            {
                _cardsOnHand = new CardsOnHand(ref user.cardsOnHand, cards.Length);
            }
            _cardsOnHand.SetCards(CARD.reverse);

            Send_Cards(cards);
        }

        public void ResetUser()
        {
            Status = STATUS.FOLD;
            user.Action.Content = "";
        }

        private void SetCardsVisibility(bool isVisible)
        {
            if (_cardsOnHand != null)
            {
                _cardsOnHand.SetVisibility(isVisible);
            }
        }

        public void RevealCards()
        {
            if (_cardsOnHand == null)
            {
                _cardsOnHand = new CardsOnHand(ref user.cardsOnHand, cards.Length);
            }
            for (int i = 0; i < cards.Length; i++)
            {
                _cardsOnHand.SetCard(i, cards[i]);
            }

        }

        private void SendMessage(string msg)
        {
            Misc.Networking.SendToRemote(RemoteClient, msg);
        }
        public void Send_StartSignal()
        {
            SendMessage(KEY_VALUE.STARTED_NEW_ROUND);
        }

        private void Send_Cards(Card[] cards)
        {
            SendMessage($"SENDING_CARDS|{cards.Length}");
            foreach (Card c in cards)
            {
                SendMessage($"{c.Value}|{c.Color}");
            }
        }

        public void Send_GameInfo(int checkValue, int bigBlindValue)
        {
            SendMessage($"{KEY_VALUE.YOUR_BALANCE}{TotalBallance}");
            SendMessage($"{KEY_VALUE.YOUR_BET}{CurrentBet}");
            SendMessage($"{KEY_VALUE.CHECK_VALUE}{checkValue}");
            SendMessage($"{KEY_VALUE.BIG_BLIND}{bigBlindValue}");
            SendMessage(KEY_VALUE.END_OF_TRANSMISSION);
        }

        public void Send_CurrentFigure()
        {
            SendMessage($"{KEY_VALUE.YOUR_FIGURE}{CurrentSet.Figure}");
        }

        public void Send_WaitingForMoveSignal()
        {
            SendMessage(KEY_VALUE.WAITING_FOR_PLAYER);
        }

        public void Send_DisconnectionSignal()
        {
            SendMessage(KEY_VALUE.DISCONNECTING_SIGNAL);
        }
    }
}
