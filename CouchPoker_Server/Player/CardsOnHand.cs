using CouchPoker_Server.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CouchPoker_Server.Player
{
    class CardsOnHand
    {
        private UserCards userCards;
        private int _cardsCount;
        public CardsOnHand(ref UserCards cards, int cardsCount)
        {
            _cardsCount = cardsCount;
            userCards = cards;
            for (int i = 0; i < cards.stackPanel.Children.Count; i++)
            {
                cards.stackPanel.Children[i].Visibility = System.Windows.Visibility.Collapsed;
            }
            for (int i = 0; i < cardsCount; i++)
            {
                cards.stackPanel.Children[i].Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void SetCard(int cardNumber, Card card)
        {
            if (cardNumber >= 0 && cardNumber < _cardsCount)
                ((Image)userCards.stackPanel.Children[cardNumber]).Source =
                    new BitmapImage(new Uri(card.Path, UriKind.Relative));
        }

        public void SetCards(string imagePath)
        {
            for (int i = 0; i < _cardsCount; i++)
            {
                ((Image)userCards.stackPanel.Children[i]).Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            }
        }

        public void SetVisibility(bool isVisible)
        {
            System.Windows.Visibility visibility =
                    (isVisible) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            userCards.stackPanel.Visibility = visibility;
        }

        public UserCards GetView()
        {
            return userCards;
        }
    }
}
