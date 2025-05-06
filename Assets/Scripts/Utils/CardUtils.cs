using UnityEngine;

public static class CardUtils
{
    
    public static bool IsOppositeColor(CardInstance a, CardInstance b)
    {

        return a.GetColor() != b.GetColor();
    }

    public static bool isSameSuit(CardInstance a, CardInstance b)
    {
        return (a.GetSuit() == b.GetSuit());
    }

    public static bool isNextValue(CardInstance lowerRankCard, CardInstance higherRankCard)
    {
        return lowerRankCard.GetValue() == higherRankCard.GetValue() - 1;
    }

    public static string GetDisplayName(CardInstance card)
    {
        string valueName = card.GetValue() switch
        {
            0 => "Wild",
            1 => "Ace",
            11 => "Jack",
            12 => "Queen",
            13 => "King",
            _ => card.GetValue().ToString()
        };

        return $"{valueName} of {card.GetSuit()}";
    }

    public static string GetDisplayValue(int cardValue)
    {
        string valueName = cardValue switch
        {
            0 => "Wild",
            1 => "Ace",
            11 => "Jack",
            12 => "Queen",
            13 => "King",
            _ => cardValue.ToString()
        };
        return valueName;
    }

    //Only really used if the color hasn't been set yet
    public static CardColor GetColorFromSuit(CardSuit suit)
    {
        switch (suit)
        {
            case CardSuit.Hearts:
            case CardSuit.Diamonds:
                return CardColor.Red;
            case CardSuit.Clubs:
            case CardSuit.Spades:
                return CardColor.Black;
            default:
                return CardColor.Gold; // Fallback or special case
        }
    }

    public static Color GetColorFromColor(CardColor color) {
        switch (color)
        {
            case CardColor.Red:
                return new Color(0.6f, 0.1f, 0.1f);
                
            case CardColor.Black:
                return new Color(0.44f, 0.5f, 0.56f);


            case CardColor.Gold:
                return new Color(1.0f, 0.84f, 0.0f, 1.0f);
               
            default:
                return new Color(1.0f, 0.84f, 0.0f, 1.0f);
        }
    }
}
