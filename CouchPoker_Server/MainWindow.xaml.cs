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
using System.Windows.Threading;

namespace CouchPoker_Server
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dispatcher dispatcher;
        public static List<UserHandler> users;
        public static List<UserData> usersHistory;
        public static List<Card> usedCards;

        public MainWindow()
        {
            InitializeComponent();
            dispatcher = this.Dispatcher;
            usedCards = new List<Card>();
            usersHistory = new List<UserData>();


            users = new List<UserHandler>()
            {
                new UserHandler(UserSlot_1, new UserData()),
                new UserHandler(UserSlot_2, new UserData()),
                new UserHandler(UserSlot_3, new UserData()),
                new UserHandler(UserSlot_4, new UserData()),
                new UserHandler(UserSlot_5, new UserData()),
                new UserHandler(UserSlot_6, new UserData()),
                new UserHandler(UserSlot_7, new UserData())
            };

            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());
            usedCards.Add(GetRandomCardSafely());

            Card[] cards = new Card[] { usedCards[0], usedCards[1], usedCards[2] };

            SetCards(GAME_MOMENT.FLOP, cards);
            SetCards(GAME_MOMENT.TURN, new Card[] { usedCards[3] });
            SetCards(GAME_MOMENT.RIVER, new Card[] { usedCards[4] });


            JoiningManagement.Run(users, usersHistory);
            RunGame();

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

        private void RunGame()
        {
            Task t = new Task(() =>
            {
                while (true)
                {
                    int current;
                    do
                    {
                        current = Dispatcher.Invoke( new Func<int>(CountUsers));
                        Console.WriteLine("waiting for at least two players to join...");
                        Console.WriteLine($"currently connected: {current}...");
                        System.Threading.Thread.Sleep(1000);
                    } while (current < 2);
                    var currentUsers = Dispatcher.Invoke(new Func<List<UserHandler>>(GetActiveUsers));
                    while (currentUsers.Count >= 2)
                    {
                        foreach(var user in currentUsers)
                        {
                            usedCards.Add(GetRandomCardSafely());
                            usedCards.Add(GetRandomCardSafely());
                            user.SetCards(usedCards[usedCards.Count-1], usedCards[usedCards.Count-2]);
                        }

                        foreach(UserHandler user in currentUsers)
                        {
                            Dispatcher.Invoke(new Action(() => { user.Status = STATUS.MY_TURN; }));
                            Dispatcher.Invoke(new Action(() => { }));
                            while (user.Status == STATUS.MY_TURN) { Console.WriteLine($"Waiting for {user.Username}..."); System.Threading.Thread.Sleep(200); }
                        }



                        Console.WriteLine("Time to play...");
                        Console.WriteLine($"currently connected: {currentUsers.Count}...");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            });
            t.Start();
        }

        private List<UserHandler> GetActiveUsers()
        {
            List<UserHandler> active = new List<UserHandler>();
            foreach (UserHandler usr in users)
            {
                if (usr.IsActive) active.Add(usr);
            }
            return active;
        }

        private int CountUsers()
        {
            int currentPlayers = 0;
            foreach (UserHandler usr in users)
            {
                if (usr.IsActive)
                {
                    currentPlayers++;
                }
            }
            return currentPlayers;
        }
    }

    public enum GAME_MOMENT
    {
        PREFLOP, FLOP, TURN, RIVER
    }
}
