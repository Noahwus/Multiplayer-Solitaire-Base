using FishNet.CodeGenerating;
using FishNet.Serializing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardMoveData
{
    public string cardID;
    public string targetSlot;
    public bool isFaceUp;

    public CardMoveData(string cardID, string targetSlot, bool isFacedUp)
    {
        this.cardID = cardID;
        this.targetSlot = targetSlot;
        this.isFaceUp = isFacedUp;
    }
}

public static class CardMoveDataSerializer
{
    public static void WriteCardMoveData(this Writer writer, CardMoveData value)
    {
        writer.WriteString(value.cardID);
        writer.WriteString(value.targetSlot);
        writer.WriteBoolean(value.isFaceUp);
    }

    public static CardMoveData ReadCardMoveData(this Reader reader)
    {
        CardMoveData data = new CardMoveData
        {
            cardID = reader.ReadStringAllocated(),
            targetSlot = reader.ReadStringAllocated(),
            isFaceUp = reader.ReadBoolean()
        };
        return data;
    }
}
