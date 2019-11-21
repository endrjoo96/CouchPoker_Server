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
        private class FigureMeta
        {
            public VALUE HIGH_CARD { get; set; }
            public VALUE LOW_CARD { get; set; }
            public Card[] figureCards { get; set; }
        }

        public List<Card> cardsSet;

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
                                    cardsSet.Remove(cardsSet[i]);
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
                                        cardsSet.Remove(cardsSet[i]);
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
                            for (int i = 0; i < 2; i++)
                            {
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
                    case FIGURE.FLUSH:
                        {
                            List<Card>[] cards = new List<Card>[]
                            {
                            new List<Card>(),
                            new List<Card>(),
                            new List<Card>(),
                            new List<Card>()
                            };

                            for (int i = 0; i < cardsSet.Count; i++)
                            {
                                switch (cardsSet[i].Color)
                                {
                                    case CARD.HEARTS: cards[0].Add(cardsSet[i]); break;
                                    case CARD.DIAMONDS: cards[1].Add(cardsSet[i]); break;
                                    case CARD.SPADES: cards[2].Add(cardsSet[i]); break;
                                    case CARD.CLUBS: cards[3].Add(cardsSet[i]); break;
                                }
                            }
                            foreach (var x in cards)
                            {
                                if (x.Count > 5)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        dynamicList.Add(x[i]);
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    case FIGURE.STRAIGHT:
                        {
                            for (int i = 0; i < cardsSet.Count - 4; i++)
                            {
                                for (int j = i; j < i + 4; j++)
                                {

                                    if (j == cardsSet.Count - 1 &&
                                        (cardsSet[j].Value_Enum == VALUE._2 && cardsSet[0].Value_Enum == VALUE._A))
                                    {
                                        if (!dynamicList.Contains(cardsSet[j])) dynamicList.Add(cardsSet[j]);
                                        if (!dynamicList.Contains(cardsSet[0])) dynamicList.Add(cardsSet[0]);
                                    }
                                    else if (cardsSet[j].Value_Enum == cardsSet[j + 1].Value_Enum + 1)
                                    {
                                        if (!dynamicList.Contains(cardsSet[j])) dynamicList.Add(cardsSet[j]);
                                        if (!dynamicList.Contains(cardsSet[j + 1])) dynamicList.Add(cardsSet[j + 1]);
                                    }
                                    else dynamicList.Clear();


                                }
                                if (dynamicList.Count == 5) break;
                            }
                            break;
                        }
                    case FIGURE.POKER:
                        {

                            for (int i = 0; i < cardsSet.Count - 1; i++)
                            {
                                List<Card>[] cards = new List<Card>[]
                                {
                            new List<Card>(),
                            new List<Card>(),
                            new List<Card>(),
                            new List<Card>()
                                };
                                if (i == cardsSet.Count - 1)
                                {
                                    for (int x = 0; x < cardsSet.Count - 1; x++)
                                    {
                                        if (cardsSet[x].Color == CARD.HEARTS) cards[0].Add(cardsSet[x]);
                                        else if (cardsSet[x].Color == CARD.DIAMONDS) cards[0].Add(cardsSet[x]);
                                        else if (cardsSet[x].Color == CARD.CLUBS) cards[0].Add(cardsSet[x]);
                                        else if (cardsSet[x].Color == CARD.SPADES) cards[0].Add(cardsSet[x]);
                                    }

                                }

                                bool straight = false;
                                foreach (var l in cards)
                                {
                                    if (l.Count >= 5)
                                    {
                                        for (int e = 0; e < l.Count - 4; e++)
                                        {
                                            for (int j = e; j < e + 4; e++)
                                            {

                                                if (j == l.Count - 1 &&
                                                    (l[j].Value_Enum == VALUE._2 && l[0].Value_Enum == VALUE._A))
                                                {
                                                    straight = true;
                                                }
                                                else if (l[j].Value_Enum == l[j + 1].Value_Enum + 1)
                                                {
                                                    straight = true;
                                                }
                                                else
                                                {
                                                    straight = false;
                                                }
                                            }
                                            if (straight)
                                            {
                                                for (int q = 0; q < l.Count - 4; q++)
                                                {
                                                    for (int j = q; j < q + 4; j++)
                                                    {

                                                        if (j == l.Count - 1 &&
                                                            (l[j].Value_Enum == VALUE._2 && l[0].Value_Enum == VALUE._A))
                                                        {
                                                            if (!dynamicList.Contains(l[j])) dynamicList.Add(l[j]);
                                                            if (!dynamicList.Contains(l[0])) dynamicList.Add(l[0]);
                                                        }
                                                        else if (cardsSet[j].Value_Enum == cardsSet[j + 1].Value_Enum + 1)
                                                        {
                                                            if (!dynamicList.Contains(l[j])) dynamicList.Add(l[j]);
                                                            if (!dynamicList.Contains(l[j + 1])) dynamicList.Add(l[j + 1]);
                                                        }
                                                        else dynamicList.Clear();
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }



                            }
                            break;
                        }
                    case FIGURE.ROYAL_POKER:
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                dynamicList.Add(cardsSet[i]);
                            }
                            break;
                        }

                        //TODO: straight, poker, royal



                }
                cardsSet = new List<Card>(dynamicList);
            }
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
            List<FIGURE> detectedFigures = new List<FIGURE>() { FIGURE.HIGH_CARD };
            for (int i = 0; i < cardsSet.Count - 1; i++)
            {
                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum)
                {
                    detectedFigures.Add(FIGURE.PAIR);
                    break;
                }
            }
            if (detectedFigures.Contains(FIGURE.PAIR))
            {
                VALUE firstPairValue = VALUE._2;
                bool firstPairDetected = false;
                for (int i = 0; i < cardsSet.Count - 1; i++)
                {
                    if (firstPairDetected)
                    {
                        if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum && cardsSet[i + 1].Value_Enum == firstPairValue)
                        {
                            if (detectedFigures.Contains(FIGURE.THREE_OF_A_KIND))
                            {
                                detectedFigures.Add(FIGURE.FOUR_OF_A_KIND);
                                Figure = FIGURE.FOUR_OF_A_KIND;
                                InsertBestCardsToList();
                                return;
                            }
                            detectedFigures.Add(FIGURE.THREE_OF_A_KIND);
                            break;
                        }
                        else if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum && cardsSet[i + 1].Value_Enum != firstPairValue)
                        {
                            detectedFigures.Add(FIGURE.TWO_PAIRS);
                        }

                    }
                    else if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum && !firstPairDetected)
                    {
                        firstPairValue = cardsSet[i].Value_Enum;
                        firstPairDetected = true;
                    }
                }
                if (detectedFigures.Contains(FIGURE.THREE_OF_A_KIND) &&
                    detectedFigures.Contains(FIGURE.PAIR))
                {
                    Figure = FIGURE.FULL;
                    InsertBestCardsToList();
                    return;
                }
                else if (detectedFigures.Contains(FIGURE.TWO_PAIRS))
                {
                    Figure = FIGURE.TWO_PAIRS;
                    InsertBestCardsToList();
                    return;
                }
            }


            bool straight = false;
            int from = 0;
            for (int i = 0; i < cardsSet.Count - 4; i++)
            {
                for (int j = i; j < i + 4; j++)
                {

                    if (j == cardsSet.Count - 1 &&
                        (cardsSet[j].Value_Enum == VALUE._2 && cardsSet[0].Value_Enum == VALUE._A))
                    {
                        straight = true;
                    }
                    else if (cardsSet[j].Value_Enum == cardsSet[j + 1].Value_Enum + 1)
                    {
                        straight = true;
                    }
                    else
                    {
                        straight = false;
                        break;
                    }
                }
                if (straight)
                {
                    from = i;
                    detectedFigures.Add(FIGURE.STRAIGHT);
                    break;
                }
            }
            if (straight)
            {
                for (int i = 0; i < cardsSet.Count - 1; i++)
                {
                    List<Card>[] cards = new List<Card>[]
                    {
                            new List<Card>(),
                            new List<Card>(),
                            new List<Card>(),
                            new List<Card>()
                    };

                    for (int x = 0; x < cardsSet.Count; x++)
                    {
                        if (cardsSet[x].Color == CARD.HEARTS) cards[0].Add(cardsSet[x]);
                        else if (cardsSet[x].Color == CARD.DIAMONDS) cards[1].Add(cardsSet[x]);
                        else if (cardsSet[x].Color == CARD.CLUBS) cards[2].Add(cardsSet[x]);
                        else if (cardsSet[x].Color == CARD.SPADES) cards[3].Add(cardsSet[x]);
                    }


                    foreach (var l in cards)
                    {
                        if (l.Count >= 5)
                        {
                            for (int e = 0; e < l.Count - 4; e++)
                            {
                                for (int j = e; j < e + 4; e++)
                                {

                                    if (j == l.Count - 1 &&
                                        (l[j].Value_Enum == VALUE._2 && l[0].Value_Enum == VALUE._A))
                                    {
                                        straight = true;
                                    }
                                    else if (l[j].Value_Enum == l[j + 1].Value_Enum + 1)
                                    {
                                        straight = true;
                                    }
                                    else
                                    {
                                        straight = false;
                                    }
                                }
                                if (straight)
                                {
                                    detectedFigures.Add(FIGURE.POKER);
                                    break;
                                }
                            }
                            straight = false;
                            if (l[0].Value_Enum == VALUE._A)
                            {
                                for (int e = 0; e < l.Count - 2; e++)
                                {
                                    if (l[e].Value_Enum == l[e].Value_Enum + 1) straight = true;
                                    else straight = false;
                                }
                                if (straight) detectedFigures.Add(FIGURE.ROYAL_POKER);
                            }



                        }
                    }
                }
            }




            detectedFigures.Sort((p, q) => q.CompareTo(p));
            Figure = detectedFigures[0];
            InsertBestCardsToList();
            return;
        }

        /*private FigureMeta DetectPair()
        {
            FigureMeta result = new FigureMeta();
            for (int i = 0; i < cardsSet.Count - 1; i++)
            {
                if (cardsSet[i].Value_Enum == cardsSet[i + 1].Value_Enum)
                {
                    result.HIGH_CARD = cardsSet[i].Value_Enum;
                    break;
                }
            }
        }*/


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
