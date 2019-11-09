using CouchPoker_Server.Management;
using CouchPoker_Server.Networking;
using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CouchPoker_Server
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<UserHandler> users;
        List<UserData> usersHistory;
        List<Card> usedCards;
        private int currentPlayer = 0;
        private bool warunek = true;

        public MainWindow()
        {
            InitializeComponent();
            usedCards = new List<Card>();
            usersHistory = new List<UserData>()
            {
                new UserData(){ ballance=0, uID="", username="" },
                new UserData(){ ballance=1000, uID="abcd-123f-123da-wesf", username="jerne" },
                new UserData(){ ballance=2900, uID="dsaw-123f-123da-wesf", username="player2" },
                new UserData(){ ballance=31245, uID="123r-123f-123da-wesf", username="xdddde" }
            };

            users = new List<UserHandler>()
            {
                new UserHandler(UserSlot_1, usersHistory[1]),
                new UserHandler(UserSlot_2, usersHistory[2]),
                new UserHandler(UserSlot_3, usersHistory[3]),
                new UserHandler(UserSlot_4, usersHistory[0]),
                new UserHandler(UserSlot_5, usersHistory[0]),
                new UserHandler(UserSlot_6, usersHistory[0]),
                new UserHandler(UserSlot_7, usersHistory[0])
            };
            users[0].Status = STATUS.MY_TURN;

            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());

            Card[] cards = new Card[] { usedCards[0], usedCards[1], usedCards[2] };

            SetCards(GAME_MOMENT.FLOP, cards);
            SetCards(GAME_MOMENT.TURN, new Card[] { usedCards[3] });
            SetCards(GAME_MOMENT.RIVER, new Card[] { usedCards[4] });


            Worker.Run();
            JoiningManagement.Run(users, usersHistory);

            /*while (warunek)
            {
                users[currentPlayer].Status = STATUS.MY_TURN;

            }*/
        }

        public Card GetRandomCardSafely()
        {
            Card c = CARD.GetRandomCard();
            while (usedCards.Find(x => x.Path == c.Path) != null)
            {
                c = CARD.GetRandomCard();
            }
            return c;
        }

        private void SetCards(GAME_MOMENT moment, Card[] cards)
        {
            switch (moment)
            {
                case GAME_MOMENT.FLOP:
                {
                        choosedCards.Flop_1.Source = new BitmapImage(new Uri(cards[0].Path, UriKind.Relative));
                        choosedCards.Flop_2.Source = new BitmapImage(new Uri(cards[1].Path, UriKind.Relative));
                        choosedCards.Flop_3.Source = new BitmapImage(new Uri(cards[2].Path, UriKind.Relative));
                        break;
                }
                case GAME_MOMENT.TURN:
                {

                        choosedCards.Turn.Source = new BitmapImage(new Uri(cards[0].Path, UriKind.Relative));
                        break;
                }
                case GAME_MOMENT.RIVER:
                {

                        choosedCards.River.Source = new BitmapImage(new Uri(cards[0].Path, UriKind.Relative));
                        break;
                }
            }

        }
    }

    public enum GAME_MOMENT
    {
        PREFLOP, FLOP, TURN, RIVER
    }
}
