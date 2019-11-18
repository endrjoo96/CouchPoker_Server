using CouchPoker_Server.Management;
using CouchPoker_Server.Networking;
using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public static DealerHandler dealer;
        public static List<UserHandler> users;
        public static List<UserData> usersHistory;
        public static List<Card> usedCards;
        public Task gameTask;
        public static CancellationTokenSource cts;
        int blindValue = 20;

        public MainWindow()
        {
            InitializeComponent();
            dealer = new DealerHandler(Dealer);
            dispatcher = this.Dispatcher;
            usedCards = new List<Card>();
            usersHistory = new List<UserData>();
            cts = new CancellationTokenSource();

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

        short currentDealerIndex = 0;
        private async void RunGame()
        {
            Task t = Task.Run(() =>
            {
                currentDealerIndex = 0;
                CancellationToken ct = cts.Token;
                ct.ThrowIfCancellationRequested();
                GAME_MOMENT currentMoment;

                while (true)
                {
                    int current;
                    do
                    {
                        current = Dispatcher.Invoke(new Func<int>(CountActiveUsers));
                        Console.WriteLine("waiting for at least two players to join...");
                        Console.WriteLine($"currently connected: {current}...");
                        System.Threading.Thread.Sleep(1000);
                    } while (current < 2);
                    var currentUsers = Dispatcher.Invoke(new Func<List<UserHandler>>(GetActiveUsers));
                    foreach (UserHandler u in currentUsers) u.IsPlaying = true;
                    while (currentUsers.Count >= 2)
                    {
                        currentMoment = GAME_MOMENT.PREFLOP;
                        while (currentMoment != GAME_MOMENT.SHOW)
                        {
                            switch (currentMoment)
                            {
                                case GAME_MOMENT.PREFLOP:
                                    {
                                        ExecutePreflop(currentUsers);
                                        break;
                                    }
                                case GAME_MOMENT.FLOP:
                                    {
                                        ExecuteFlop();
                                        break;
                                    }
                                case GAME_MOMENT.TURN:
                                    {
                                        ExecuteTurn();
                                        break;
                                    }
                                case GAME_MOMENT.RIVER:
                                    {
                                        ExecuteRiver();
                                        break;
                                    }
                            }
                            UserHandler potentialWinner = Bidd(currentUsers);
                            if (potentialWinner == null)
                                currentMoment++;
                        }
                        ExecuteShow();








                        foreach (UserHandler user in currentUsers)
                        {
                            Dispatcher.Invoke(new Action(() => { user.Status = STATUS.MY_TURN; }));
                            Dispatcher.Invoke(new Action(() => { }));
                            while (user.Status == STATUS.MY_TURN)
                            {
                                Console.WriteLine($"Waiting for {user.Username}...");
                                System.Threading.Thread.Sleep(1000);
                            }
                        }



                        Console.WriteLine("Time to play...");
                        Console.WriteLine($"currently connected: {currentUsers.Count}...");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }, cts.Token);

            try
            {
                await t;
            }
            catch (OperationCanceledException ocex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {
                cts.Dispose();
            }
        }
        private UserHandler Bidd(List<UserHandler> currentUsers)
        {
            int checkValue = 0;
            UserHandler winner = null;
            Dispatcher.Invoke(() =>
            {
                bool begin = false;
                int checkedUsersCount = 0;
                bool exit = false;
                while (!exit)
                {
                    foreach (UserHandler u in currentUsers)
                    {
                        if (u.Status != STATUS.FOLD || u.Status != STATUS.ALL_IN)
                        {
                            if (u.Status == STATUS.DEALER)
                            {
                                u.Status = STATUS.NO_ACTION;
                                begin = true;
                                continue;
                            }
                            else if (u.Status == STATUS.SMALL_BLIND)
                            {
                                if (u.TotalBallance < (blindValue / 2)) u.Status = STATUS.FOLD;
                                else
                                {
                                    u.TotalBallance -= (blindValue / 2);
                                    dealer.Pot += (blindValue / 2);
                                    u.CurrentBet = (blindValue / 2);
                                }
                            }
                            else if (u.Status == STATUS.BIG_BLIND)
                            {
                                if (u.TotalBallance < (blindValue)) u.Status = STATUS.FOLD;
                                else
                                {
                                    u.TotalBallance -= (blindValue);
                                    dealer.Pot += (blindValue);
                                    u.CurrentBet = (blindValue);
                                }
                            }

                            else if (begin)
                            {
                                u.Status = STATUS.MY_TURN;
                                while (u.Status == STATUS.MY_TURN)
                                {
                                    if (u.IsActive)
                                    {
                                        Console.WriteLine($"Waiting for {u.Username}...");
                                        Thread.Sleep(1000);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{u.Username} disconnected, skipping...");
                                        u.Status = STATUS.FOLD;
                                    }
                                }
                                switch (u.Status)
                                {
                                    case STATUS.CHECK:
                                        {
                                            checkedUsersCount++;
                                            int coinsToAdd = checkValue - u.CurrentBet;
                                            u.TotalBallance -= coinsToAdd;
                                            dealer.Pot += coinsToAdd;
                                            u.CurrentBet = checkValue;
                                            break;
                                        }
                                    case STATUS.BET:
                                        {
                                            checkedUsersCount = 0;
                                            int coinsToAdd = u.latestArgs.value - u.CurrentBet;
                                            u.TotalBallance -= coinsToAdd;
                                            dealer.Pot += coinsToAdd;
                                            checkValue = u.latestArgs.value;
                                            u.CurrentBet = checkValue;
                                            break;
                                        }
                                    case STATUS.FOLD:
                                        {
                                            break;
                                        }
                                }
                                int nonFoldedUsers = 0;
                                foreach(var u2 in currentUsers)
                                {
                                    if (u2.Status != STATUS.FOLD) nonFoldedUsers++;
                                }
                                if (checkedUsersCount == nonFoldedUsers)
                                {
                                    exit = true;
                                    break;
                                }
                                if (nonFoldedUsers==1)
                                {
                                    exit = true;
                                    foreach (var u2 in currentUsers)
                                    {
                                        if (u2.Status != STATUS.FOLD) winner = u2;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            });
            return winner;
        }

        private void ExecutePreflop(List<UserHandler> currentUsers)
        {
            foreach (var user in currentUsers)
            {
                usedCards.Add(GetRandomCardSafely());
                usedCards.Add(GetRandomCardSafely());
                user.SetCards(new Card[] { usedCards[usedCards.Count - 1], usedCards[usedCards.Count - 2] });
                user.Status = STATUS.NEW_GAME;
                user.IsDealer = false;
            }
            for (int i = 0; i < 3; i++)
            {
                int currentPlace = currentDealerIndex + i;
                if (currentPlace > currentUsers.Count - 1) currentPlace = 0 + (currentPlace - (currentUsers.Count));
                if (i == 0) currentUsers[currentPlace].Status = STATUS.DEALER;
                else if (i == 1)
                {
                    currentUsers[currentPlace].Status = STATUS.SMALL_BLIND;
                }
                else if (i == 2)
                {
                    currentUsers[currentPlace].Status = STATUS.BIG_BLIND;
                }

            }
        }

        private void ExecuteFlop()
        {

        }

        private void ExecuteTurn()
        {

        }

        private void ExecuteRiver()
        {

        }

        private void ExecuteShow()
        {

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

        public static int CountActiveUsers()
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

        public static void NotEnoughPlayers()
        {
            cts.Cancel();
        }
    }

    public enum GAME_MOMENT
    {
        PREFLOP, FLOP, TURN, RIVER, SHOW
    }
}
