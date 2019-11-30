using CouchPoker_Server.Management;
using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static CouchPoker_Server.Misc.Threading;

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
            
            usedCards = new List<Card>()
            {
                new Card(CARD.HEARTS, CARD._A),
                new Card(CARD.DIAMONDS, CARD._2),
                new Card(CARD.CLUBS, CARD._3),
                new Card(CARD.SPADES, CARD._4),
                new Card(CARD.DIAMONDS, CARD._5),
            };

            var backupUserCardsList = new List<Card>(usedCards);

            var myCards = new List<Card>(usedCards);

            List<Set> sets = new List<Set>();

            var cards = new List<Card>(myCards); 
            Set s1 = new Set(new List<Card>(cards));
            string msg2 = "Karty: ";
            for (int a = 0; a < cards.Count; a++)
            {
                msg2 += $"{cards[a].Value}{cards[a].Color}, ";
            }
            msg2 += $"\nWykryta figura: {s1.Figure.ToString()}\nKarty układu: ";
            for (int a = 0; a < s1.cardsSet.Count; a++)
            {
                msg2 += $"{s1.cardsSet[a].Value}{s1.cardsSet[a].Color}, ";
            }
            MessageBox.Show(msg2);

            string msg;
            do
            {
                usedCards = new List<Card>(backupUserCardsList);
                List<Card> c = new List<Card>(myCards);
                usedCards.Add(GetRandomCardSafely());
                usedCards.Add(GetRandomCardSafely());
                c.Add(usedCards[usedCards.Count - 1]);
                c.Add(usedCards[usedCards.Count - 2]);
                Set set = new Set(new List<Card>(c));
                sets.Add(set);
                msg = "Karty: ";
                for (int a = 0; a < c.Count; a++)
                {
                    msg += $"{c[a].Value}{c[a].Color}, ";
                }
                msg += $"\nWykryta figura: {set.Figure.ToString()}\nKarty układu: ";
                for (int a = 0; a < set.cardsSet.Count; a++)
                {
                    msg += $"{set.cardsSet[a].Value}{set.cardsSet[a].Color}, ";
                }
            } while (MessageBox.Show(msg, "Karty", MessageBoxButton.OKCancel) == MessageBoxResult.OK);

            throw new NotImplementedException("dosc");

            /*for (int i=0;i<10;)
            {
                List<Card> c = new List<Card>(myCards);
                usedCards.Add(GetRandomCardSafely());
                usedCards.Add(GetRandomCardSafely());
                c.Add(usedCards[usedCards.Count - 1]);
                c.Add(usedCards[usedCards.Count - 2]);
                Set set = new Set(new List<Card>(c));
                sets.Add(set);
                string msg = "Karty: ";
                for(int a=0; a<c.Count; a++)
                {
                    msg += $"{c[a].Value}{c[a].Color}, ";
                }
                msg += $"\nWykryta figura: {set.Figure.ToString()}\nKarty układu: ";
                for (int a = 0; a < set.cardsSet.Count; a++)
                {
                    msg += $"{set.cardsSet[a].Value}{set.cardsSet[a].Color}, ";
                }
                MessageBox.Show(msg);
                i++;
            }*/

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
                case GAME_MOMENT.PREFLOP:
                    {
                        ImageSource src = new BitmapImage(new Uri(CARD.reverse, UriKind.Relative));
                        choosedCards.Flop_1.Source = src;
                        choosedCards.Flop_2.Source = src;
                        choosedCards.Flop_3.Source = src;
                        choosedCards.Turn.Source = src;
                        choosedCards.River.Source = src;
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
                    List<UserHandler> currentUsers;
                    do
                    {
                        currentUsers = Dispatcher.Invoke(new Func<List<UserHandler>>(GetActiveUsers));
                        foreach (UserHandler u in currentUsers)
                        {
                            InvokeIfRequired(() =>
                            {
                                u.IsPlaying = true;
                                u.IsDealer = false;
                                u.Status = STATUS.NEW_GAME;

                            });
                        }
                        InvokeIfRequired(() => { usedCards = new List<Card>(); });

                        if (currentDealerIndex >= currentUsers.Count)
                        {
                            currentDealerIndex = 0;
                        }

                        currentMoment = GAME_MOMENT.PREFLOP;
                        while (currentMoment != GAME_MOMENT.SHOW)
                        {
                            foreach (UserHandler u in currentUsers)
                            {
                                InvokeIfRequired(() => { u.CurrentBet = 0; });
                            }

                            switch (currentMoment)
                            {
                                case GAME_MOMENT.PREFLOP:
                                    {
                                        Dispatcher.Invoke(() => { ExecutePreflop(currentUsers); });
                                        break;
                                    }
                                case GAME_MOMENT.FLOP:
                                    {
                                        Dispatcher.Invoke(() => { ExecuteFlop(); });

                                        break;
                                    }
                                case GAME_MOMENT.TURN:
                                    {
                                        Dispatcher.Invoke(() => { ExecuteTurn(); });

                                        break;
                                    }
                                case GAME_MOMENT.RIVER:
                                    {
                                        Dispatcher.Invoke(() => { ExecuteRiver(); });

                                        break;
                                    }
                            }
                            UserHandler potentialWinner = Bidd(currentUsers);
                            if (potentialWinner == null)
                                currentMoment++;
                            else break;
                        }
                        ExecuteShow(currentUsers);
                        ExecuteRewardCalculation(currentUsers);

                        currentDealerIndex++;

                        foreach (UserHandler u in currentUsers) u.IsPlaying = false;


                        
                        Console.WriteLine("Time to play...");
                        Console.WriteLine($"currently connected: {currentUsers.Count}...");
                        System.Threading.Thread.Sleep(1000);
                    } while (currentUsers.Count >= 2);
                }
            }, cts.Token);

            try
            {
                await t;
            }
            catch (OperationCanceledException ocex)
            {
                Console.WriteLine(ocex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
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
                            InvokeIfRequired(() =>
                            {
                                u.Status = STATUS.NO_ACTION;
                            });
                            begin = true;
                            continue;
                        }
                        else if (u.Status == STATUS.SMALL_BLIND)
                        {
                            if (u.TotalBallance < (blindValue / 2))
                                InvokeIfRequired(() =>
                                {
                                    u.Status = STATUS.FOLD;
                                    u.IsPlaying = false;
                                });
                            else
                            {
                                InvokeIfRequired(() =>
                                {
                                    u.TotalBallance -= (blindValue / 2);
                                    dealer.Pot += (blindValue / 2);
                                    u.CurrentBet = (blindValue / 2);
                                    u.Status = STATUS.NO_ACTION;
                                });
                            }
                        }
                        else if (u.Status == STATUS.BIG_BLIND)
                        {
                            if (u.TotalBallance < (blindValue))
                                InvokeIfRequired(() =>
                                {
                                    u.Status = STATUS.FOLD;
                                    u.IsPlaying = false;
                                });
                            else
                            {
                                InvokeIfRequired(() =>
                                {
                                    u.TotalBallance -= (blindValue);
                                    dealer.Pot += (blindValue);
                                    u.CurrentBet = (blindValue);
                                    u.Status = STATUS.NO_ACTION;
                                    checkValue = blindValue;
                                });
                            }
                            begin = true;
                        }
                        

                        else if (begin)
                        {
                            InvokeIfRequired(() =>
                            {

                                u.Status = STATUS.MY_TURN;
                            });
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
                                        InvokeIfRequired(() =>
                                        {

                                            int coinsToAdd = checkValue - u.CurrentBet;
                                            u.TotalBallance -= coinsToAdd;
                                            dealer.Pot += coinsToAdd;
                                            u.CurrentBet = checkValue;
                                        });
                                        break;
                                    }
                                case STATUS.BET:
                                    {
                                        checkedUsersCount = 1;
                                        InvokeIfRequired(() =>
                                        {

                                            int coinsToAdd = u.latestArgs.value - u.CurrentBet;
                                            u.TotalBallance -= coinsToAdd;
                                            dealer.Pot += coinsToAdd;
                                            checkValue = u.latestArgs.value;
                                            u.CurrentBet = checkValue;
                                        });
                                        break;
                                    }
                                case STATUS.FOLD:
                                    {
                                        break;
                                    }
                            }
                            int nonFoldedUsers = 0;
                            foreach (var u2 in currentUsers)
                            {
                                if (u2.Status != STATUS.FOLD) nonFoldedUsers++;
                            }
                            if (checkedUsersCount == nonFoldedUsers)
                            {
                                exit = true;
                                break;
                            }
                            if (nonFoldedUsers == 1)
                            {
                                exit = true;
                                foreach (var u2 in currentUsers)
                                {
                                    if (u2.Status != STATUS.FOLD) winner = u2;
                                }
                                break;
                            }
                        }
                        else if (u.IsDealer) begin = true;
                    }
                }
            }
            return winner;
        }

        private void ExecutePreflop(List<UserHandler> currentUsers)
        {
            SetCards(GAME_MOMENT.PREFLOP, null);
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
            for (int i = 0; i < 3; i++) { usedCards.Add(GetRandomCardSafely()); }
            SetCards(GAME_MOMENT.FLOP, new Card[]
            {
                usedCards[usedCards.Count-3],
                usedCards[usedCards.Count-2],
                usedCards[usedCards.Count-1]
            });
        }

        private void ExecuteTurn()
        {
            usedCards.Add(GetRandomCardSafely());
            SetCards(GAME_MOMENT.TURN, new Card[]
            {
                usedCards[usedCards.Count-1]
            });
        }

        private void ExecuteRiver()
        {
            usedCards.Add(GetRandomCardSafely());
            SetCards(GAME_MOMENT.RIVER, new Card[]
            {
                usedCards[usedCards.Count-1]
            });
        }

        private void ExecuteShow(List<UserHandler> currentUsers)
        {
            foreach (UserHandler u in currentUsers)
            {
                if (u.Status != STATUS.FOLD)
                {
                    InvokeIfRequired(() => { u.RevealCards(); });
                }
            }

        }

        private void ExecuteRewardCalculation(List<UserHandler> currentUsers)
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
