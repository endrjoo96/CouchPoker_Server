using CouchPoker_Server.Management;
using CouchPoker_Server.Networking;
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
    public partial class MainWindow : Window
    {
        public static string servername;
        public static Dispatcher dispatcher;
        public static DealerHandler dealer;
        public static List<UserHandler> users;
        public static List<UserData> usersHistory;
        public static List<Card> usedCards;
        public List<Card> publicCards;
        public Task gameTask;
        public Broadcaster broadcaster;
        private static volatile bool runGame_break = false;

        public static int blindValue = 20;
        public static int startupTokens = 1000;
        public MainWindow()
        {
            InitializeComponent();

            SettingsWindow settings = new SettingsWindow();
            if (settings.ShowDialog() == true)
            {
                servername = settings.ServerName;
                blindValue = 2 * settings.SmallBlind;
                startupTokens = settings.StartupTokens;
            }
            else System.Windows.Application.Current.Shutdown();

            dealer = new DealerHandler(Dealer);
            dispatcher = this.Dispatcher;
            usedCards = new List<Card>();
            usersHistory = new List<UserData>() {
                new UserData("testUUID_1", "Player1", startupTokens),
                new UserData("testUUID_2", "Player2", startupTokens),
                new UserData("testUUID_3", "Player3", startupTokens), 
                new UserData("testUUID_4", "Player4", startupTokens),
            };

            users = new List<UserHandler>()
            {
                new UserHandler(UserSlot_1, new UserData(), 2),
                new UserHandler(UserSlot_2, new UserData(), 2),
                new UserHandler(UserSlot_3, new UserData(), 2),
                new UserHandler(UserSlot_4, new UserData(), 2),
                new UserHandler(UserSlot_5, new UserData(), 2),
                new UserHandler(UserSlot_6, new UserData(), 2),
                new UserHandler(UserSlot_7, new UserData(), 2),
                new UserHandler(UserSlot_8, new UserData(), 2),
                new UserHandler(UserSlot_9, new UserData(), 2),
                new UserHandler(UserSlot_10, new UserData(), 2),
                new UserHandler(UserSlot_11, new UserData(), 2),
                new UserHandler(UserSlot_12, new UserData(), 2),
                new UserHandler(UserSlot_13, new UserData(), 2),
                new UserHandler(UserSlot_14, new UserData(), 2),
                new UserHandler(UserSlot_15, new UserData(), 2)
            };
            JoiningManagement.Run(users, usersHistory);
            broadcaster = new Broadcaster();

            this.Title = $"This is server: {servername} @ {broadcaster.GetIPAddress()}";

            RunGame();
        }

        private Card GetRandomCardSafely()
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
                    choosedCards.Flop_1.Source = new BitmapImage(new Uri(cards[0].Path, UriKind.Relative)); publicCards.Add(cards[0]);
                    choosedCards.Flop_2.Source = new BitmapImage(new Uri(cards[1].Path, UriKind.Relative)); publicCards.Add(cards[1]);
                    choosedCards.Flop_3.Source = new BitmapImage(new Uri(cards[2].Path, UriKind.Relative)); publicCards.Add(cards[2]);
                    break;
                }
                case GAME_MOMENT.TURN:
                {

                    choosedCards.Turn.Source = new BitmapImage(new Uri(cards[0].Path, UriKind.Relative)); publicCards.Add(cards[0]);
                    break;
                }
                case GAME_MOMENT.RIVER:
                {

                    choosedCards.River.Source = new BitmapImage(new Uri(cards[0].Path, UriKind.Relative)); publicCards.Add(cards[0]);
                    break;
                }
                case GAME_MOMENT.PREFLOP:
                {
                    publicCards = new List<Card>();
                    ImageSource src = new BitmapImage(new Uri(CARD.reverse, UriKind.Relative));
                    choosedCards.Flop_1.Source = src;
                    choosedCards.Flop_2.Source = src;
                    choosedCards.Flop_3.Source = src;
                    choosedCards.Turn.Source = src;
                    choosedCards.River.Source = src;
                    break;
                }
                case GAME_MOMENT.GONE:
                {
                    choosedCards.Flop_1.Source = null;
                    choosedCards.Flop_2.Source = null;
                    choosedCards.Flop_3.Source = null;
                    choosedCards.Turn.Source = null;
                    choosedCards.River.Source = null;
                    break;
                }
            }

        }

        short currentDealerIndex = 0;
        private async void RunGame()
        {
            Task t = new Task(() =>
            {
                currentDealerIndex = 0;
                GAME_MOMENT currentMoment;
                UserHandler potentialWinner = null;

                while (true)
                {
                    InvokeIfRequired(() => { ResetBoard(); });
                    int current;
                    do
                    {
                        current = Dispatcher.Invoke(new Func<int>(CountActiveUsers));
                        System.Threading.Thread.Sleep(1000);
                    } while (current < 2);
                    List<UserHandler> currentUsers;
                    int activeUsersCount;
                    do
                    {
                        runGame_break = false;
                        skipBidd = false;
                        InvokeIfRequired(() => { PrepareBoard(); });
                        activeUsersCount = 0;
                        currentUsers = Dispatcher.Invoke(new Func<List<UserHandler>>(GetActiveUsers));
                        foreach (UserHandler u in currentUsers)
                        {
                            InvokeIfRequired(() =>
                            {
                                u.IsPlaying = true;
                                u.IsDealer = false;
                                u.Status = STATUS.NEW_GAME;
                                u.Send_StartSignal();
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
                                if (u.IsPlaying)
                                    InvokeIfRequired(() =>
                                    {
                                        u.CurrentBet = 0;
                                        u.Status = STATUS.NO_ACTION;
                                    });
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
                                    Dispatcher.Invoke(() => { ExecuteFlop(currentUsers); });
                                    break;
                                }
                                case GAME_MOMENT.TURN:
                                {
                                    Dispatcher.Invoke(() => { ExecuteTurn(currentUsers); });
                                    break;
                                }
                                case GAME_MOMENT.RIVER:
                                {
                                    Dispatcher.Invoke(() => { ExecuteRiver(currentUsers); });
                                    break;
                                }
                            }
                            potentialWinner = Bidd(currentUsers);
                            int stillPlaying = 0;
                            foreach (var u in currentUsers)
                            {
                                if (u.IsPlaying) stillPlaying++;
                            }
                            if (potentialWinner != null || stillPlaying < 2)
                                break;
                            else currentMoment++;
                        }
                        UserHandler[] winners;
                        if (potentialWinner == null)
                        {
                            winners = ExecuteRewardCalculation(currentUsers);
                        }
                        else
                            winners = new UserHandler[] { potentialWinner };
                        ExecuteShow(currentUsers);
                        System.Threading.Thread.Sleep(2000);

                        foreach (UserHandler w in winners) { InvokeIfRequired(() => { w.Status = STATUS.WINNER; }); }
                        if (currentMoment == GAME_MOMENT.SHOW)
                        {
                            if (winners.Length == 1)
                            {
                                SetDealerMessage($"{winners[0].Username} won ${dealer.Pot}\nwith {winners[0].CurrentSet.Figure.ToString()}");
                            }
                            else
                            {
                                string msg = "";
                                for (int i = 0; i < winners.Length; i++)
                                {
                                    if (i != 0) msg += " and ";
                                    msg += winners[i].Username;
                                }
                                SetDealerMessage($"{msg} won ${((int)dealer.Pot/winners.Length)} each\nwith {winners[0].CurrentSet.Figure.ToString()}");
                            }
                        }

                        System.Threading.Thread.Sleep(2000);

                        InvokeIfRequired(() => { RewardWinners(dealer, winners); });
                        System.Threading.Thread.Sleep(2000);

                        foreach (UserHandler u in currentUsers) u.IsPlaying = false;
                        currentDealerIndex++;

                        foreach (var u in users)
                        {
                            if (u.IsActive && u.TotalBallance>0) activeUsersCount++;
                        }
                    }
                    while (activeUsersCount >= 2);
                }
            });
            t.Start();

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
        }

        bool skipBidd = false;
        private UserHandler Bidd(List<UserHandler> currentUsers)
        {
            int checkValue = 0;
            UserHandler winner = null;
            bool begin = false;
            int checkedUsersCount = 0;
            bool exit = false;
            while (!exit && !skipBidd)
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

                        else if (u.Status == STATUS.FOLD ||
                            u.Status == STATUS.NEW_USER ||
                            u.Status == STATUS.ALL_IN ||
                            !u.IsActive || !u.IsPlaying) continue;

                        else if (begin)
                        {
                            foreach (UserHandler usr in currentUsers)
                            {
                                if (usr.IsActive && usr.IsPlaying)
                                {
                                    InvokeIfRequired(() =>
                                    {
                                        usr.Send_GameInfo(checkValue, blindValue);
                                    });
                                }
                            }

                            InvokeIfRequired(() =>
                            {
                                u.Status = STATUS.MY_TURN;
                            });
                            u.Send_WaitingForMoveSignal();

                            bool amIOnlyActiveUser = true;
                            foreach (UserHandler usr in GetPlayingUsers())
                            {
                                if (!(usr.Status == STATUS.ALL_IN || usr.Status == STATUS.MY_TURN))
                                {
                                    amIOnlyActiveUser = false;
                                    break;
                                }
                            }
                            if (amIOnlyActiveUser) skipBidd = true;

                            while (u.Status == STATUS.MY_TURN)
                            {
                                if (u.IsActive)
                                {
                                    if (!runGame_break)
                                        Thread.Sleep(500);
                                    else
                                    {
                                        InvokeIfRequired(() => { u.Status = STATUS.NO_ACTION; });
                                        winner = u;
                                    }
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
                                case STATUS.RAISE:
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
                            if (u.TotalBallance == 0)
                            {
                                InvokeIfRequired(() => { u.Status = STATUS.ALL_IN; });
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

        private void ExecuteFlop(List<UserHandler> currentUsers)
        {
            for (int i = 0; i < 3; i++) { usedCards.Add(GetRandomCardSafely()); }
            SetCards(GAME_MOMENT.FLOP, new Card[]
            {
                usedCards[usedCards.Count-3],
                usedCards[usedCards.Count-2],
                usedCards[usedCards.Count-1]
            });
            foreach (UserHandler u in currentUsers)
            {
                List<Card> cards = new List<Card>(publicCards);
                cards.Add(u.cards[0]);
                cards.Add(u.cards[1]);
                u.CurrentSet = new Set(cards);
            }
        }

        private void ExecuteTurn(List<UserHandler> currentUsers)
        {
            usedCards.Add(GetRandomCardSafely());
            SetCards(GAME_MOMENT.TURN, new Card[]
            {
                usedCards[usedCards.Count-1]
            });
            foreach (UserHandler u in currentUsers)
            {
                List<Card> cards = new List<Card>(publicCards);
                cards.Add(u.cards[0]);
                cards.Add(u.cards[1]);
                u.CurrentSet = new Set(cards);
            }
        }

        private void ExecuteRiver(List<UserHandler> currentUsers)
        {
            usedCards.Add(GetRandomCardSafely());
            SetCards(GAME_MOMENT.RIVER, new Card[]
            {
                usedCards[usedCards.Count-1]
            });
            foreach (UserHandler u in currentUsers)
            {
                List<Card> cards = new List<Card>(publicCards);
                cards.Add(u.cards[0]);
                cards.Add(u.cards[1]);
                u.CurrentSet = new Set(cards);
            }
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

        private UserHandler[] ExecuteRewardCalculation(List<UserHandler> currentUsers)
        {
            List<UserHandler> userList = new List<UserHandler>();
            foreach (UserHandler u in currentUsers)
            {
                if (u.IsPlaying || u.Status != STATUS.FOLD) userList.Add(u);
            }
            if(userList.Count>=2) 
                userList.Sort((p, q) => q.CurrentSet.Figure.CompareTo(p.CurrentSet.Figure));
            List<UserHandler> usersWithConflictingSets = new List<UserHandler>() { userList[0] };

            for (int i = 1; i < userList.Count; i++)
            {
                if (userList[i].CurrentSet.Figure == userList[0].CurrentSet.Figure) usersWithConflictingSets.Add(userList[i]);
            }

            int iterator = 0;
            while (usersWithConflictingSets.Count > 1 && iterator < 5)
            {
                int count = usersWithConflictingSets.Count;
                usersWithConflictingSets.Sort(
                    (p, q) => q.CurrentSet.cardsSet[iterator].Value_Enum.CompareTo(
                        p.CurrentSet.cardsSet[iterator].Value_Enum));
                for (int i = 0; i < count - 1; i++)
                {
                    if (usersWithConflictingSets[i].CurrentSet.cardsSet[iterator].Value_Enum !=
                        usersWithConflictingSets[i + 1].CurrentSet.cardsSet[iterator].Value_Enum)
                    {
                        usersWithConflictingSets.RemoveAt(i + 1);
                        count--;
                        i--;
                    }
                }
                iterator++;
            }

            return usersWithConflictingSets.ToArray();
        }

        private void RewardWinners(DealerHandler dealer, UserHandler[] winners)
        {
            int rewardValue = dealer.Pot / winners.Length;

            foreach (var winner in winners)
            {
                winner.TotalBallance += rewardValue;
            }
            dealer.Pot = 0;
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
                if (usr.IsActive && usr.TotalBallance>0)
                {
                    currentPlayers++;
                }
            }
            return currentPlayers;
        }

        public static int CountPlayingUsers()
        {
            /*int currentPlayers = 0;
            foreach (UserHandler usr in users)
            {
                if (usr.IsPlaying)
                {
                    currentPlayers++;
                }
            }*/
            return GetPlayingUsers().Count;
        }

        public static List<UserHandler> GetPlayingUsers()
        {
            List<UserHandler> playingUsers = new List<UserHandler>();
            foreach (UserHandler usr in users)
            {
                if (usr.IsPlaying && usr.IsActive)
                {
                    playingUsers.Add(usr);
                }
            }
            return playingUsers;
        }

        public static void NotEnoughPlayers()
        {
            runGame_break = true;
        }

        private void ResetBoard()
        {
            dealer.Message = "Waiting for at least 2 players to start...";
            SetCards(GAME_MOMENT.GONE, null);

            foreach (var u in users)
            {
                if (u.IsActive)
                {
                    u.ResetUser();
                }
            }
        }

        private void SetDealerMessage(string message)
        {
            InvokeIfRequired(() => { dealer.Message = message; });
        }

        private void PrepareBoard()
        {
            dealer.State = DealerHandler.DEALER_STATE.SHOW_POT;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach(UserHandler u in users)
            {
                if(u.IsActive)
                    u.Send_DisconnectionSignal();
            }
        }
    }

    public enum GAME_MOMENT
    {
        PREFLOP, FLOP, TURN, RIVER, SHOW, GONE
    }
}
