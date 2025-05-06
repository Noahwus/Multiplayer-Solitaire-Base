using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardTemplate
{
    public string name;
    public CardColor color;
    public CardSuit suit;
    public int value;

    public CardTemplate(string name, CardColor color, CardSuit suit, int value)
    {
        this.name = name;
        this.color = color;
        this.suit = suit;
        this.value = value;
    }
}
[System.Serializable]
public enum CardColor { Red, Black, Gold } //Standard Colors + Wild Card Color
[System.Serializable]
public enum CardSuit { Clubs, Diamonds, Hearts, Spades, Wild } //Wild Cards will operate Differently. Use: StandardCardSuits for standard