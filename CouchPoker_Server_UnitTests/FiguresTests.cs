using CouchPoker_Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CouchPoker_Server_UnitTests
{
    [TestClass]
    public class FiguresTest
    {
        private static int repeats = 5000;


        [TestMethod]
        public void FigureTest_HIGHCARD()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                List<VALUE> restrictedValues = new List<VALUE>();
                for (int i = 0; i < 5; i++)
                {
                    Card newcard = GenerateCardWithout(restrictedValues.ToArray());
                    cards.Add(newcard);
                    restrictedValues.Add(newcard.Value_Enum);
                    restrictedValues.Add(newcard.Value_Enum + ((i % 2 == 1) ? 1 : -1));

                }
                bool sameColor = false;
                for (int i = 0; i < 4; i++)
                {
                    if (cards[i].Color == cards[i + 1].Color) sameColor = true;
                    else
                    {
                        sameColor = false;
                        break;
                    }
                }
                if (sameColor)
                {
                    var type = cards[0].GetType();
                    FieldInfo fieldInfo = type.GetField("_color", BindingFlags.NonPublic | BindingFlags.Instance);
                    fieldInfo.SetValue(cards[0], (cards[0].Color == CARD.CLUBS) ? CARD.DIAMONDS : CARD.CLUBS);

                }
                Set s = new Set(cards);

                bool straight = false;
                for (int i = 0; i < 4; i++)
                {
                    if (s.cardsSet[i].Value_Enum == s.cardsSet[i + 1].Value_Enum + 1) straight = true;
                    else
                    {
                        straight = false;
                        break;
                    }
                }
                if (straight)
                {
                    continue;
                }

                TestCorrection(cards, s, FIGURE.HIGH_CARD, tests);
            }
        }

        [TestMethod]
        public void FigureTest_PAIR()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                Card card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[tests % 13]);
                Card card2 = new Card(CARD.COLOR[(tests + 1) % 3], card1.Value);
                cards.Add(card1);
                cards.Add(card2);
                List<VALUE> restrictedValues = new List<VALUE>() { card1.Value_Enum };
                for (int i = 0; i < 3; i++)
                {
                    Card newcard = GenerateCardWithout(restrictedValues.ToArray());
                    cards.Add(newcard);
                    restrictedValues.Add(newcard.Value_Enum);

                }
                Set s = new Set(cards);

                TestCorrection(cards, s, FIGURE.PAIR, tests);
            }
        }

        [TestMethod]
        public void FigureTest_TWO_PAIRS()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                Card card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[tests % 13]);
                Card card2 = new Card(CARD.COLOR[(tests + 2) % 3], card1.Value);
                cards.Add(card1);
                cards.Add(card2);
                List<VALUE> restrictedValues = new List<VALUE>() { card1.Value_Enum };

                Card card3 = new Card(CARD.COLOR[(tests + 1) % 3], CARD.VALUE[(tests + 1) % 13]);
                Card card4 = new Card(CARD.COLOR[(tests + 3) % 3], card3.Value);
                cards.Add(card3);
                cards.Add(card4);
                restrictedValues.Add(card3.Value_Enum);
                for (int i = 0; i < 1; i++)
                {
                    Card newcard = GenerateCardWithout(restrictedValues.ToArray());
                    cards.Add(newcard);
                    restrictedValues.Add(newcard.Value_Enum);

                }
                Set s = new Set(cards);

                TestCorrection(cards, s, FIGURE.TWO_PAIRS, tests);
            }
        }

        [TestMethod]
        public void FigureTest_THREE_OF_A_KIND()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                Card card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[tests % 13]);
                Card card2 = new Card(CARD.COLOR[(tests + 1) % 3], card1.Value);
                Card card3 = new Card(CARD.COLOR[(tests + 2) % 3], card1.Value);
                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                List<VALUE> restrictedValues = new List<VALUE>() { card1.Value_Enum };

                for (int i = 0; i < 2; i++)
                {
                    Card newcard = GenerateCardWithout(restrictedValues.ToArray());
                    cards.Add(newcard);
                    restrictedValues.Add(newcard.Value_Enum);

                }
                Set s = new Set(cards);

                TestCorrection(cards, s, FIGURE.THREE_OF_A_KIND, tests);
            }
        }

        [TestMethod]
        public void FigureTest_FOUR_OF_A_KIND()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                Card card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[tests % 13]);
                Card card2 = new Card(CARD.COLOR[(tests + 1) % 3], card1.Value);
                Card card3 = new Card(CARD.COLOR[(tests + 2) % 3], card1.Value);
                Card card4 = new Card(CARD.COLOR[(tests + 3) % 3], card1.Value);
                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);
                List<VALUE> restrictedValues = new List<VALUE>() { card1.Value_Enum };

                for (int i = 0; i < 1; i++)
                {
                    Card newcard = GenerateCardWithout(restrictedValues.ToArray());
                    cards.Add(newcard);
                    restrictedValues.Add(newcard.Value_Enum);

                }
                Set s = new Set(cards);

                TestCorrection(cards, s, FIGURE.FOUR_OF_A_KIND, tests);
            }
        }

        [TestMethod]
        public void FigureTest_FULL()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                Card card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[tests % 13]);
                Card card2 = new Card(CARD.COLOR[(tests + 1) % 3], card1.Value);
                cards.Add(card1);
                cards.Add(card2);

                Card card3 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[(tests + 1) % 13]);
                Card card4 = new Card(CARD.COLOR[(tests + 1) % 3], card3.Value);
                Card card5 = new Card(CARD.COLOR[(tests + 2) % 3], card3.Value);
                cards.Add(card3);
                cards.Add(card4);
                cards.Add(card5);

                Set s = new Set(cards);

                TestCorrection(cards, s, FIGURE.FULL, tests);
            }
        }

        [TestMethod]
        public void FigureTest_STRAIGHT()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                Card card1, card2, card3, card4, card5;
                if (tests % 100 < 98)
                {
                    card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[tests % 9]);
                    card2 = new Card(CARD.COLOR[(tests + 1) % 3], CARD.VALUE[((tests) % 9) + 1]);
                    card3 = new Card(CARD.COLOR[(tests + 2) % 3], CARD.VALUE[((tests) % 9) + 2]);
                    card4 = new Card(CARD.COLOR[(tests + 3) % 3], CARD.VALUE[((tests) % 9) + 3]);
                    card5 = new Card(CARD.COLOR[(tests + 4) % 3], CARD.VALUE[((tests) % 9) + 4]);
                }
                else
                {
                    card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[12]);
                    card2 = new Card(CARD.COLOR[(tests + 1) % 3], CARD.VALUE[0]);
                    card3 = new Card(CARD.COLOR[(tests + 2) % 3], CARD.VALUE[1]);
                    card4 = new Card(CARD.COLOR[(tests + 3) % 3], CARD.VALUE[2]);
                    card5 = new Card(CARD.COLOR[(tests + 4) % 3], CARD.VALUE[3]);
                }

                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);
                cards.Add(card5);

                Set s = new Set(cards);

                TestCorrection(cards, s, FIGURE.STRAIGHT, tests);
            }

        }

        [TestMethod]
        public void FigureTest_POKER()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                Card card1, card2, card3, card4, card5;
                if (tests % 100 < 98)
                {
                    card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[tests % 8]);
                    card2 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[((tests) % 8) + 1]);
                    card3 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[((tests) % 8) + 2]);
                    card4 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[((tests) % 8) + 3]);
                    card5 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[((tests) % 8) + 4]);
                }
                else
                {
                    card1 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[12]);
                    card2 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[0]);
                    card3 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[1]);
                    card4 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[2]);
                    card5 = new Card(CARD.COLOR[tests % 3], CARD.VALUE[3]);
                }

                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);
                cards.Add(card5);

                Set s = new Set(cards);

                TestCorrection(cards, s, FIGURE.POKER, tests);
            }

        }

        [TestMethod]
        public void FigureTest_ROYAL_POKER()
        {
            List<Card> cards = new List<Card>();
            Card card1, card2, card3, card4, card5;

            card1 = new Card(CARD.COLOR[0], CARD.VALUE[12]);
            card2 = new Card(CARD.COLOR[0], CARD.VALUE[11]);
            card3 = new Card(CARD.COLOR[0], CARD.VALUE[10]);
            card4 = new Card(CARD.COLOR[0], CARD.VALUE[9]);
            card5 = new Card(CARD.COLOR[0], CARD.VALUE[8]);

            cards.Add(card1);
            cards.Add(card2);
            cards.Add(card3);
            cards.Add(card4);
            cards.Add(card5);

            Set s = new Set(cards);

            TestCorrection(cards, s, FIGURE.ROYAL_POKER, 0);
        }

        [TestMethod]
        public void FigureTest_FLUSH()
        {
            for (int tests = 0; tests < repeats; tests++)
            {
                List<Card> cards = new List<Card>();
                List<VALUE> restrictedValues = new List<VALUE>();
                for (int i = 0; i < 5; i++)
                {
                    Card newcard = GenerateCardWithout(restrictedValues.ToArray(), CARD.COLOR[tests%3]);
                    cards.Add(newcard);
                    restrictedValues.Add(newcard.Value_Enum);
                    restrictedValues.Add(newcard.Value_Enum + ((i % 2 == 1) ? 1 : -1));

                }
                Set s = new Set(cards);

                bool straight = false;
                for (int i = 0; i < 4; i++)
                {
                    if (s.cardsSet[i].Value_Enum == s.cardsSet[i + 1].Value_Enum + 1) straight = true;
                    else
                    {
                        straight = false;
                        break;
                    }
                }
                if (straight)
                {
                    continue;
                }

                TestCorrection(cards, s, FIGURE.FLUSH, tests);
            }
        }









        /* MISC */

            public static bool EqualValues(Card c, VALUE[] restrictedValues)
        {
            foreach (VALUE v in restrictedValues)
            {
                if (c.Value_Enum == v) return true;
            }
            return false;
        }

        public static bool EqualColors(Card c, string[] restrictedColors)
        {
            foreach (string color in restrictedColors)
            {
                if (c.Color == color) return true;
            }
            return false;
        }

        public static Card GenerateCardWithout(VALUE[] restrictedValues)
        {
            Card result;
            do
            {
                result = CARD.GetRandomCard();
            } while (EqualValues(result, restrictedValues));
            return result;
        }

        public static Card GenerateCardWithout(VALUE[] restrictedValues, string withColor)
        {
            Card result;
            do
            {
                result = CARD.GetRandomCard();
            } while (EqualValues(result, restrictedValues) || result.Color != withColor);
            return result;
        }

        public static Card GenerateCardsWithout(string[] restrictedColors)
        {
            Card result;
            do
            {
                result = CARD.GetRandomCard();
            } while (EqualColors(result, restrictedColors));
            return result;

        }

        public static string GenerateStatistics(List<Card> cards, Set set, FIGURE requiredFigure, int currentTest)
        {
            string msg;
            msg = $"Error occoured on test {currentTest}. Passed cards:";
            foreach (var c in cards)
            {
                msg += $" {c.Value}{c.Color} ";
            }
            msg += $"Detected figure: {set.Figure} Cards on figure set: ";
            foreach (var c in set.cardsSet)
            {
                msg += $" {c.Value}{c.Color} ";
            }
            return msg;
        }

        public static void TestCorrection(List<Card> cards, Set set, FIGURE requiredFigure, int currentTest)
        {

            if (set.Figure != requiredFigure)
            {
                Assert.Fail($"Figure {requiredFigure} not detected. {GenerateStatistics(cards, set, requiredFigure, currentTest)}");
            }
            else if (set.cardsSet.Count != 5)
            {
                Assert.Fail($"Not enough cards in set deck. {GenerateStatistics(cards, set, requiredFigure, currentTest)}");
            }
        }
    }
}
