using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// SlotNode is our Gameobject. This is where card's will aim to go when sent to a "CardSlot", how we know which cardslots are what type, and to know what
///     card stack offsets are necessary to display the Cards correctly
/// </summary>

public enum SlotType { Deck, Discard, Play, Score, Recycle, Unknown }

public class SlotNode : MonoBehaviour
{
    public SlotData  Slot;
    public Transform cardAnchor; 
    public SlotType  slotType = SlotType.Unknown;
    public BoxCollider col;

    private void Awake()
    {
        slotType = GetSlotTypeFromTag();
        if (col == null) { col = GetComponentInChildren<BoxCollider>(); }
        
    }

    public void Initialize(SlotData sd)
    {
        Slot = sd;
    }
    public SlotType GetSlotTypeFromTag()
    {
        switch (tag)
        {
            case "Deck":    return SlotType.Deck;
            case "Discard": return SlotType.Discard;
            case "Score":   return SlotType.Score;
            case "Play":    return SlotType.Play;
            case "Recycle": return SlotType.Recycle;
            default:        return SlotType.Unknown;
        }
    }

    public Vector3 GetOffsetPosition(string cardID)
    {
        int indexInSlot = Slot.GetCardIndex(cardID);
        int cardCount = Slot.cardIDs.Count;
        int cardPadCount = SlotManager.Instance.cardPaddingCount;
        Vector3 Offset = Vector3.zero;

        float zOffset = 0f;

        if (slotType == SlotType.Play) 
        {
            float maxDist = SlotManager.Instance.maxPaddingDistance;
            zOffset = -SlotManager.Instance.cardPadding;

            if (cardCount > cardPadCount)
            {
                float zOffsetter = maxDist / cardCount;
                zOffset = -zOffsetter; 
            }
        }
        Offset = new Vector3(0, SlotManager.Instance.yOffGlobal * indexInSlot, zOffset * indexInSlot);
        return transform.position + Offset;
    }

    public void ToggleCollider(bool active)
    {
        col.enabled = active;
    }
   
}
