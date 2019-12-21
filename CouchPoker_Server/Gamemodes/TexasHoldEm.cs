using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CouchPoker_Server.Misc.Threading;

namespace CouchPoker_Server.Gamemodes
{
    public class TexasHoldEm : Gamemode
    {
        private List<UserHandler> users;
        private List<UserData> usersHistory;
        public TexasHoldEm(List<UserHandler> users, List<UserData> usersHistory)
        {
            this.users = users;
            this.usersHistory = usersHistory;
        }

        int currentDealerIndex = 0;
        public override async void RunGame()
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
                                SetDealerMessage($"{msg} won ${((int)dealer.Pot / winners.Length)} each\nwith {winners[0].CurrentSet.Figure.ToString()}");
                            }
                        }

                        System.Threading.Thread.Sleep(2000);

                        InvokeIfRequired(() => { RewardWinners(dealer, winners); });
                        System.Threading.Thread.Sleep(2000);

                        foreach (UserHandler u in currentUsers) u.IsPlaying = false;
                        currentDealerIndex++;

                        foreach (var u in users)
                        {
                            if (u.IsActive) activeUsersCount++;
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
    }
}
