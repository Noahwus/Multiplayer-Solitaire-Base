using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public class SlotData 
{
    public int      ownerID;
    public int      slotIndex;
    public SlotType slotType; public SlotType GetSlotType() => slotType;
    public string   slotID;

    public List<string> cardIDs = new List<string>();
    // private int _version; // potentail anticheat? check sync version, or revert state...

    public SlotData( int ownerid, int index, SlotType Slottype)
    {
        this.slotType   = Slottype;
        this.ownerID    = ownerid;
        this.slotIndex  = index;
        slotID = $"{slotType.ToString() + slotIndex}_{ownerid}";
    }

    public void AddCard(string cardID)
    {
        if (string.IsNullOrEmpty(cardID))
        {
            Debug.LogError($"Tried to add null cardID to {slotID}");
            return;
        }
        if (cardIDs.Contains(cardID))
        {
            Debug.LogWarning($"Card {cardID} already exists in {slotID}, cannot be added...");
            return;
        }

        cardIDs.Add(cardID);
    }

    public void RemoveCard(string cardID)
    {
        if (string.IsNullOrEmpty(cardID))
        {
            Debug.LogError($"Tried to remove null cardID to {slotID}");
            return;
        }
        if (!cardIDs.Contains(cardID))
        {
            Debug.LogWarning($"Card {cardID} does not exist in {slotID}, cannot be removed...");
            return;
        }

        cardIDs.Remove(cardID);
    }

    public List<string> GetAllCards() => (cardIDs);
    public List<Card> GetAllCardsCards()
    {
        List<Card> cards = new List<Card>();
        foreach (string cardID in cardIDs)
        {
            Card cardcard = CardManager.Instance.GetCard(cardID);
            if (cardcard != null) { cards.Add(cardcard); }
        }
        return cards;
    }


    public void SetAllCards(List<string> newCardIDs) 
    {
        cardIDs.Clear();
        cardIDs = newCardIDs; 
    }

    public void Shuffle(int seed = -1)
    {
        if(seed == -1) { DeckUtils.Shuffle(cardIDs); }
        else { DeckUtils.ShuffleWithSeed(cardIDs, seed); } 
    }

    public bool HasCards() {
        if (cardIDs.Count > 0) { return true; }
        else return false;
    }

    public string GetTopCardID()
    {
        if (cardIDs.Count <= 0) return null;
        string top = cardIDs[^1];
        //cardIDs.RemoveAt(cardIDs.Count - 1);
        return top;
    }
    public Card GetTopCard()
    {
        if (cardIDs.Count <= 0) return null;
        string top = cardIDs[^1];
        Card topCard = CardManager.Instance.GetCard(top);
        return topCard;
    }

    public int GetCardIndex(string cardID)
    {
        if (string.IsNullOrEmpty(cardID)) { Debug.Log("Card is empty!"); return -1; }
        if (!HasCards()) { Debug.Log($"{slotID} has no cards?...");      return -1; }

        if (cardIDs.Contains(cardID)) { return cardIDs.IndexOf(cardID); }

        return -1;
    }

    public string PrintCardIDs()
    {
        if (cardIDs == null || cardIDs.Count == 0)
        {
            return($"{slotID} has no cards.");
            
        }
        string buffer = ("Cards in this slot:\n");
        for (int i = 0; i < cardIDs.Count; i++)
        {
            buffer += ($"[{i}] {cardIDs[i]}\n");
        }
        return(buffer);
    }

}
