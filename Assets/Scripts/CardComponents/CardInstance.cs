using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CardInstance
{
    /// <summary>
    /// CardInstance operates as an instance of "Card Template", which is created via CardFactory.cs
    /// </summary>
   
    public int ownerID;
    public string cardID;
    public bool isFaceUp;
    public CardTemplate template;
    public List<CardModifier> modifiers = new();
    private string cardDebugName;

    public string GetDisplayName() => template.name;
    public CardColor GetColor() => template.color;
    public CardSuit GetSuit()   => template.suit;
    public int GetValue()       => template.value;
    public string GetCardID()   => cardID;
    public string GetDebugName() => cardDebugName;

    public bool isWild()
    {
        if(GetColor() == CardColor.Gold || GetSuit() == CardSuit.Wild) return true;
        return false;
    }
    
    public bool HasModifier(string name)
    {
        return modifiers.Any(mod => mod.modifierName == name && mod.isActive);
    }

    public void AddModifier(CardModifier mod)
    {
        modifiers.Add(mod);
    }

    public CardInstance(CardTemplate template, int ownerID, bool isFaceUp = false)
    {
        this.template = template;
        this.ownerID = ownerID;
        this.isFaceUp = isFaceUp;
        cardID = template.name + "_" + ownerID;
        cardDebugName = $"{CardUtils.GetDisplayValue(GetValue())}_of_{Utils.ColorText(GetSuit().ToString(), CardUtils.GetColorFromColor(GetColor()))}_{ownerID.ToString()}";
    }
}
