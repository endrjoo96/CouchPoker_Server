using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server
{
    public class Card
    {
        private string _path;
        private string _color;
        private string _value;

        public string Path { get { return _path; } }
        public string Color { get { return _color; } }
        public string Value { get { return _value; } }

        public Card(string c, string v)
        {
            _color = c;
            _value = v;
            _path = $"{CARD._path}/{v}{c}.jpg";
        }
    }

    public static class CARD
    {
        public static string _path = @"/CouchPoker_Server;component/Resources/Cards";

        private const string
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
            Card c = new Card(COLOR[r.Next(0, 3)], VALUE[r.Next(0,12)]);
            return c;
        }
    }
}
