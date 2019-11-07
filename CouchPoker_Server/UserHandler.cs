﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CouchPoker_Server
{
    enum STATUS
    {
        CHECK, BET, FOLD, NO_ACTION, NEW_GAME, MY_TURN
    }

    class UserHandler
    {

        private Controls.User user = null;
        private STATUS _status = STATUS.NEW_GAME;

        public TcpClient tcpClient { get; set; }
        public string ID { get; set; }

        public string Username {
            get { return user.Username.Content.ToString(); }
            set { if (user.Username.Content.ToString() == "")
                    user.Username.Content = value;
                user.Visibility = System.Windows.Visibility.Visible;
            }
        }
        public int TotalBallance {
            get { return Int32.Parse(user.Total.Content.ToString()); }
            set { user.Total.Content = value.ToString(); }
        }
        public int CurrentBet {
            get { return Int32.Parse(user.Current.Content.ToString()); }
            set { user.Current.Content = value.ToString(); }
        }
        public STATUS Status {
            get { return _status; }
            set { switch (value) {
                    case STATUS.FOLD:
                    {
                        ChangeColor(new SolidColorBrush(Colors.Gray));
                        user.Action.Content = value;
                        break;
                    }
                    case STATUS.NO_ACTION:
                    case STATUS.NEW_GAME:
                    {
                        ChangeColor(new SolidColorBrush(Colors.White));
                        break;
                    }
                    case STATUS.MY_TURN:
                    {
                        user.Username.Foreground = new SolidColorBrush(Colors.Yellow);
                        user.Action.Content = "";
                        break;
                    }
                }
                _status = value;
            }
        }

        public UserHandler(Controls.User uiControl, string username)
        {
            user = uiControl;
            Username = username;
            if (Username == "") uiControl.Visibility = System.Windows.Visibility.Hidden;
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

        public STATUS Play()
        {
            Status = STATUS.MY_TURN;
            return STATUS.BET;
        }

    }
}
