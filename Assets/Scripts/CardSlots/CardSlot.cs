using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CardSlot operates as a hub, connecting Gameobjects (Node) and Slot Data (Data)
/// </summary>

public class CardSlot : MonoBehaviour
{    
    public SlotData Data { get; private set; }
    public SlotNode Node { get; private set; }

    private string _slotID; public string GetSlotID() => _slotID;

    public SlotType Initialize(int playerID, int index)
    {
        this.Node = GetComponent<SlotNode>();
             Node.GetSlotTypeFromTag();

        this.Data = new SlotData(
            playerID,
            index,
            Node.slotType);

        Node.Initialize(Data);

        _slotID = $"{Data.slotIndex}_{Data.slotType}_{Data.ownerID}";
        return Node.slotType;
    }

    public bool HasCard(Card card)
    {
        if (card == null || card.Data == null) return false;
        return Data.cardIDs.Contains(card.Data.GetCardID());
    }

    public bool HasCards()
    {
        return Data.cardIDs.Count > 0;
    }

    public Card GetTopCard()
    {
        return Data.GetTopCard();
    }

}
