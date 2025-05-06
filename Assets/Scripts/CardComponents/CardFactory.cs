using System.Collections.Generic;
using UnityEngine;

public static class CardFactory
{
    public static List<CardSuit> standardSuits = new List<CardSuit>
    {
        CardSuit.Clubs,
        CardSuit.Diamonds,
        CardSuit.Hearts,
        CardSuit.Spades
    };

    public static List<CardInstance> CreateDeckFromTemplates(List<CardTemplate> templates, int ownerID = -1)
    {
        List<CardInstance> deck = new(52);

        foreach (var template in templates)
        {
            deck.Add(new CardInstance(template, ownerID));
        }

        return deck;
    }

    public static List<CardTemplate> CreateStandardCardTemplates()
    {
        List<CardTemplate> templates = new(52);

        foreach (CardSuit suit in standardSuits) //System.Enum.GetValues(typeof(CardSuit))
        {
            CardColor color = CardUtils.GetColorFromSuit(suit);

            for (int value = 1; value <= 13; value++)
            {
                string name = $"{CardUtils.GetDisplayValue(value)}_of_{suit.ToString()}";
                templates.Add(new CardTemplate(name, color, suit, value));
            }
        }
        return templates;
    }

    public static CardInstance CreateWildCard(int ownerID = -1, List<CardModifier> modifiers = null)
    {
        CardTemplate wildTemplate = new CardTemplate("Wild", CardColor.Gold, CardSuit.Wild, 0); // Value 0 and Spades as neutral placeholder
        CardInstance wildCard = new CardInstance(wildTemplate, ownerID);

        if (modifiers != null)
            wildCard.modifiers.AddRange(modifiers);

        return wildCard;
    }

    public static List<CardTemplate> CreateCardTemplates(int[] values, CardSuit[] cardSuits)
    {
        List<CardTemplate> templates = new();

        foreach (CardSuit suit in cardSuits)
        {
            CardColor color = CardUtils.GetColorFromSuit(suit);

            foreach (int value in values)
            {
                string name = $"{CardUtils.GetDisplayValue(value)}_of_{suit.ToString()}";
                templates.Add(new CardTemplate(name, color, suit, value));
            }
        }

        return templates;
    }

}
