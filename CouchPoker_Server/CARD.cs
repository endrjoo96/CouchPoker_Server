using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server
{
    public enum FIGURE
    {
        HIGH_CARD, PAIR, TWO_PAIRS, THREE_OF_A_KIND, STRAIGHT, FLUSH, FULL, FOUR_OF_A_KIND, POKER, ROYAL_POKER
    }

    public enum VALUE
    {
        _2, _3, _4, _5, _6, _7, _8, _9, _10, _J, _Q, _K, _A
    }

    public class Set
    {
        private List<Card> cardsSet;

        public FIGURE Figure { get; private set; } = FIGURE.HIGH_CARD;

        public Set(Card[] set) : this(new List<Card>(set)) { }
        public Set(List<Card> set)
        {
            cardsSet = set;
            SortCards();
            CalculateFigure();
        }

        private void InsertBestCardsToList(FIGURE figure)
        {
            if (cardsSet.Count > 5)
            {
                var dynamicList = new List<Card>();
                switch (figure)
                {
                    case FIGURE.HIGH_CARD:
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                dynamicList.Add(cardsSet[i]);
                            }
                            break;
                        }
                    case FIGURE.PAIR:
                        {
                            for (int i = 0; i < cardsSet.Count - 1; i++)
                            {
                                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum)
                                {
                                    dynamicList.Add(cardsSet[i]);
                                    dynamicList.Add(cardsSet[i + 1]);
                                    cardsSet.Remove(cardsSet[i]);
                                    cardsSet.Remove(cardsSet[i + 1]);
                                    break;
                                }
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                dynamicList.Add(cardsSet[i]);
                            }
                            break;
                        }
                    case FIGURE.TWO_PAIRS:
                        {
                            for (int x = 0; x < 2; x++)
                                for (int i = 0; i < cardsSet.Count - 1; i++)
                                {
                                    if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum)
                                    {
                                        dynamicList.Add(cardsSet[i]);
                                        dynamicList.Add(cardsSet[i + 1]);
                                        cardsSet.Remove(cardsSet[i]);
                                        cardsSet.Remove(cardsSet[i + 1]);
                                        break;
                                    }
                                }
                            dynamicList.Add(cardsSet[0]);
                            break;
                        }
                    case FIGURE.THREE_OF_A_KIND:
                        {
                            for (int i = 0; i < cardsSet.Count - 2; i++)
                            {
                                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum &&
                                    cardsSet[i+1].Value_Enum == cardsSet[i + 2].Value_Enum)
                                {
                                    dynamicList.Add(cardsSet[i]);
                                    dynamicList.Add(cardsSet[i + 1]);
                                    dynamicList.Add(cardsSet[i + 2]);
                                    cardsSet.Remove(cardsSet[i]);
                                    cardsSet.Remove(cardsSet[i + 1]);
                                    cardsSet.Remove(cardsSet[i + 2]);
                                    break;
                                }
                            }
                            for(int i = 0; i < 2; i++){
                                dynamicList.Add(cardsSet[i]);
                            }
                            break;
                        }
                    case FIGURE.FOUR_OF_A_KIND:
                        {
                            for (int i = 0; i < cardsSet.Count - 3; i++)
                            {
                                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum &&
                                    cardsSet[i + 1].Value_Enum == cardsSet[i + 2].Value_Enum &&
                                    cardsSet[i + 2].Value_Enum == cardsSet[i + 3].Value_Enum)
                                {
                                    dynamicList.Add(cardsSet[i]);
                                    dynamicList.Add(cardsSet[i + 1]);
                                    dynamicList.Add(cardsSet[i + 2]);
                                    dynamicList.Add(cardsSet[i + 3]);
                                    cardsSet.Remove(cardsSet[i]);
                                    cardsSet.Remove(cardsSet[i + 1]);
                                    cardsSet.Remove(cardsSet[i + 2]);
                                    cardsSet.Remove(cardsSet[i + 3]);
                                    break;
                                }
                            }
                            dynamicList.Add(cardsSet[0]);
                            break;
                        }
                    case FIGURE.FULL:
                        {
                            for (int i = 0; i < cardsSet.Count - 2; i++)
                            {
                                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum &&
                                    cardsSet[i + 1].Value_Enum == cardsSet[i + 2].Value_Enum)
                                {
                                    dynamicList.Add(cardsSet[i]);
                                    dynamicList.Add(cardsSet[i + 1]);
                                    dynamicList.Add(cardsSet[i + 2]);
                                    cardsSet.Remove(cardsSet[i]);
                                    cardsSet.Remove(cardsSet[i + 1]);
                                    cardsSet.Remove(cardsSet[i + 2]);
                                    break;
                                }
                            }
                            for (int i = 0; i < cardsSet.Count - 1; i++)
                            {
                                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum &&
                                    cardsSet[i + 1].Value_Enum == cardsSet[i + 2].Value_Enum)
                                {
                                    dynamicList.Add(cardsSet[i]);
                                    dynamicList.Add(cardsSet[i + 1]);
                                    cardsSet.Remove(cardsSet[i]);
                                    cardsSet.Remove(cardsSet[i + 1]);
                                    break;
                                }
                            }
                            break;
                        }


                        //TODO: straight, flush, poker, royal



                }
                cardsSet = dynamicList;
            }






            throw new NotImplementedException();
        }

        private void InsertBestCardsToList()
        {
            InsertBestCardsToList(Figure);
        }

        private void SortCards()
        {
            cardsSet.Sort((p, q) => q.Value_Enum.CompareTo(p.Value_Enum));
        }
        private void CalculateFigure()
        {
            //checking for pairs
            FIGURE[] detectedFigures = new FIGURE[] { FIGURE.HIGH_CARD };
            for (int i = 0; i < cardsSet.Count - 1; i++)
            {
                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum)
                {
                    detectedFigures = new FIGURE[] { FIGURE.PAIR };
                    break;
                }
            }
            if (detectedFigures[0] == FIGURE.PAIR)
            {
                VALUE firstPairValue = VALUE._2;
                bool firstPairDetected = false;
                for (int i = 0; i < cardsSet.Count - 1; i++)
                {
                    if (firstPairDetected)
                    {
                        if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum && cardsSet[i + 1].Value_Enum == firstPairValue)
                        {
                            if (detectedFigures[0] == FIGURE.TWO_PAIRS)
                            {
                                detectedFigures = new FIGURE[] { FIGURE.TWO_PAIRS, FIGURE.THREE_OF_A_KIND };
                                break;
                            }
                            else if (detectedFigures[0] == FIGURE.THREE_OF_A_KIND)
                            {
                                detectedFigures = new FIGURE[] { FIGURE.FOUR_OF_A_KIND };
                                Figure = FIGURE.FOUR_OF_A_KIND;
                                InsertBestCardsToList();
                                return;
                            }
                            else detectedFigures = new FIGURE[] { FIGURE.THREE_OF_A_KIND };
                        }
                        else if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum && cardsSet[i + 1].Value_Enum != firstPairValue)
                        {
                            if (detectedFigures[0] == FIGURE.THREE_OF_A_KIND)
                            {
                                detectedFigures = new FIGURE[] { FIGURE.TWO_PAIRS, FIGURE.THREE_OF_A_KIND };
                                break;
                            }
                            else detectedFigures = new FIGURE[] { FIGURE.TWO_PAIRS };
                        }

                    }
                    else if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum && !firstPairDetected)
                    {
                        firstPairValue = cardsSet[i].Value_Enum;
                        firstPairDetected = true;
                    }
                }
                if (detectedFigures.Length == 2)
                {
                    Figure = FIGURE.FULL;
                    InsertBestCardsToList();
                    return;
                }
                else if (detectedFigures[0] == FIGURE.TWO_PAIRS)
                {
                    Figure = FIGURE.TWO_PAIRS;
                    InsertBestCardsToList();
                    return;
                }
                else if (detectedFigures[0] == FIGURE.THREE_OF_A_KIND)
                {
                    Figure = FIGURE.THREE_OF_A_KIND;
                    InsertBestCardsToList();
                    return;
                }


            }
            else
            {




            }



            throw new NotImplementedException();
        }
    }
    public class Card
    {
        private string _path;
        private string _color;
        private string _value;
        private VALUE _value_enum;

        public string Path { get { return _path; } private set { _path = value; } }
        public string Color { get { return _color; } private set { _color = value; } }
        public string Value
        {
            get { return _value; }
            private set
            {
                _value = value;
                _value_enum = (VALUE)Enum.Parse(typeof(VALUE), "_" + value);
            }
        }
        public VALUE Value_Enum { get { return _value_enum; } }

        public Card(string color, string value)
        {
            Color = color;
            Value = value;
            Path = $"{CARD.path}/{value}{color}.jpg";
        }
    }

    public static class CARD
    {
        public static string path = @"/CouchPoker_Server;component/Resources/Cards";
        public static string reverse = path + "/Red_back.jpg";

        public const string
            _2 = "2",
            _3 = "3",
            _4 = "4",
            _5 = "5",
            _6 = "6",
            _7 = "7",
            _8 = "8",
            _9 = "9",
            _10 = "10",
            _J = "J",
            _Q = "Q",
            _K = "K",
            _A = "A";

        public const string
            HEARTS = "H",
            DIAMONDS = "D",
            CLUBS = "C",
            SPADES = "S";

        public static string[] VALUE = new string[] { _2, _3, _4, _5, _6, _7, _8, _9, _10, _J, _Q, _K, _A };
        public static string[] COLOR = new string[] { HEARTS, DIAMONDS, CLUBS, SPADES };
        public static Card GetRandomCard()
        {
            Random r = new Random();
            Card c = new Card(COLOR[r.Next(0, 3)], VALUE[r.Next(0, 12)]);
            return c;
        }

        public static Card GetStrongest(List<Card[]> setsOfCards)
        {





            throw new NotImplementedException();
        }
    }
}
