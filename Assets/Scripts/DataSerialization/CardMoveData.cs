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

/*
 using FishNet.CodeGenerating;
using FishNet.Serializing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents moves including string[] cardID, string[] targetSlot, bool[] isFaceUp.
/// </summary>
[UseGlobalCustomSerializer]
public struct CardMoveData
{
    public string[] cardID;
    public string[] targetSlot;
    public bool[] isFaceUp;

    public CardMoveData(string[] cardIDs, string[] targetSlotID, bool[] isFaceUps)
    {
        this.cardID = cardIDs;
        this.targetSlot = targetSlotID;
        this.isFaceUp = isFaceUps;
    }
}

public static class CardMoveDataSerializer
{
    // Write the new CardMoveData (for multiple cards)
    public static void WriteCardMoveData(this Writer writer, CardMoveData value)
    {
        // Ensure all arrays have the same length before serializing
        if (value.cardID.Length != value.targetSlot.Length || value.cardID.Length != value.isFaceUp.Length)
        {
            Debug.LogError("Card arrays length mismatch!");
            return;
        }

        // Writing length of arrays
        writer.WriteInt32(value.cardID.Length);  // Write the length of cardID array
        foreach (var cardID in value.cardID)
        {
            writer.WriteString(cardID); // Write each cardID
        }

        // Writing targetSlot array
        foreach (var slotID in value.targetSlot)
        {
            writer.WriteString(slotID); // Write each targetSlot
        }

        // Writing isFaceUp array
        foreach (var isFace in value.isFaceUp)
        {
            writer.WriteBoolean(isFace);  // Write each isFaceUp value
        }
    }

    // Read the new CardMoveData (for multiple cards)
    public static CardMoveData ReadCardMoveData(this Reader reader)
    {
        // Read the lengths of each array
        int cardCount = reader.ReadInt32();  // Read length for arrays

        // Read the arrays for cardIDs, targetSlot IDs, and face-up states
        string[] cardIDs = new string[cardCount];
        for (int i = 0; i < cardCount; i++)
        {
            cardIDs[i] = reader.ReadStringAllocated();  // Read each cardID
        }

        string[] targetSlots = new string[cardCount];
        for (int i = 0; i < cardCount; i++)
        {
            targetSlots[i] = reader.ReadStringAllocated();  // Read each targetSlot
        }

        bool[] isFaceUps = new bool[cardCount];
        for (int i = 0; i < cardCount; i++)
        {
            isFaceUps[i] = reader.ReadBoolean();  // Read each isFaceUp value
        }

        // Return the deserialized CardMoveData
        return new CardMoveData(cardIDs, targetSlots, isFaceUps);
    }
}


 
 */