using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server
{
    public enum CARD_COLOR
    {
        HEARTS, DIAMONDS, CLUBS, SPADES
    }
    public enum CARD_VALUE
    {
        _2, _3, _4, _5, _6, _7, _8, _9, _10, _J, _Q, _K, _A
    }

    public class Card
    {
        string path;
        CARD_COLOR Color;
        CARD_VALUE Value;

        public Card(CARD_COLOR c, CARD_VALUE v)
        {
            Color = c;
            Value = v;
            path = $"{CARD._path}\\{Enum.GetName(typeof(CARD_COLOR), c)}{Enum.GetName(typeof(CARD_VALUE), v)}";
        }
    }

    public static class CARD
    {
        public static string _path = @"G:\Kod\CouchPoker_Server\CouchPoker_Server\bin\Debug\Cards";
        public static Card GetCard()
        {
            return null;
        }
    }
}
