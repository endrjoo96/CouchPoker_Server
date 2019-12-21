using CouchPoker_Server.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CouchPoker_Server.Management
{
    public class Board
    {
        private List<UserHandler> users;
        public readonly DealerHandler dealer;
        private List<Card> choosedCards;
        private List<Image> choosedCardsUI;

        public Board(int cardsForUsers, Controls.User[] users, Controls.Dealer dealer, Controls.ChoosedCards cards) : this(cardsForUsers, users, dealer)
        {
            choosedCards = new List<Card>();
            choosedCardsUI = new List<Image>()
            {
                cards.Flop_1, cards.Flop_2, cards.Flop_3, cards.Turn, cards.River
            };
        }
        public Board(int cardsForUsers, Controls.User[] users, Controls.Dealer dealer)
        {
            this.users = new List<UserHandler>();
            this.dealer = new DealerHandler(dealer);

            for (int i = 0; i < users.Length; i++)
            {
                this.users.Add(new UserHandler(users[i], new Player.UserData(), cardsForUsers));
            }

        }

        public void SetCards(int[] cardsIndexes, Card[] cards)
        {
            if (cardsIndexes.Length == cards.Length)
                for (int i = 0; i < cardsIndexes.Length; i++)
                {
                    SetCard(cardsIndexes[i], cards[i]);
                }
        }

        public void SetCard(int index, Card card)
        {
            if (index >= 0 && index <= 4)
            {
                if (choosedCards.Count == 5)
                    choosedCards[index] = card;
                else choosedCards.Add(card);
                choosedCardsUI[index].Source = card.GetImage();
            }
        }

        public void SetCards(int[] cardsIndexes, Card card)
        {
            for (int i = 0; i < cardsIndexes.Length; i++)
            {
                SetCard(cardsIndexes[i], card);
            }
        }
    }
}
