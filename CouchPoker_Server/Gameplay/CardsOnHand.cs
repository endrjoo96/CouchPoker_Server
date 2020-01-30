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
        private List<Image> cardsImages;
        public CardsOnHand(ref UserCards cards, int cardsCount)
        {
            _cardsCount = cardsCount;
            userCards = cards;
            cardsImages = new List<Image>()
            {
                userCards.CARD_1, userCards.CARD_2, userCards.CARD_3, userCards.CARD_4, userCards.CARD_5,
            };
            for (int i = 0; i < cardsImages.Count; i++)
            {
                cardsImages[i].Visibility = System.Windows.Visibility.Collapsed;
            }
            for (int i = 0; i < cardsCount; i++)
            {
                cardsImages[i].Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void SetCard(int cardNumber, Card card)
        {
            if (cardNumber >= 0 && cardNumber < _cardsCount)
                ((Image)cardsImages[cardNumber]).Source =
                    new BitmapImage(new Uri(card.Path, UriKind.Relative));
        }

        public void SetCards(string imagePath)
        {
            for (int i = 0; i < _cardsCount; i++)
            {
                ((Image)cardsImages[i]).Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            }
        }

        public void SetVisibility(bool isVisible)
        {
            System.Windows.Visibility visibility =
                    (isVisible) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            userCards.mainPanel.Visibility = visibility;
        }

        public UserCards GetView()
        {
            return userCards;
        }
    }
}
