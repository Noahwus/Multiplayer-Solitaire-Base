using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card : MonoBehaviour
{
    private string _cardID;

    public CardInstance Data { get; private set; }
    public CardVisual CardVisuals;

    public Draggable dragger;
    public Moveable mover;

    private void Awake()
    {
        dragger = GetComponent<Draggable>();
        mover = GetComponent<Moveable>();
    }

    public string Initialize(CardInstance data) //, CardVisual visuals
    {
        Data = data;
        if (CardVisuals == null) { CardVisuals = GetComponent<CardVisual>(); }

        _cardID = data.GetCardID();
        return _cardID;
    }

    public void AssignOwnership(int playerID)
    {
        //Assign ownership for FishNet here...
    }

    public bool HasModifier(string name)
    {
        return Data.HasModifier(name);
    }

    public void AddModifier(CardModifier modifier)
    {
        Data.AddModifier(modifier);
        // Optionally update visuals here
    }

    public bool isBurried()
    {
        CardSlot cs = SlotManager.Instance.GetSlotByCard(this);
        int indexInSlot = cs.Data.GetCardIndex(_cardID);

        return indexInSlot != -1 && indexInSlot < cs.Data.cardIDs.Count - 1;
    }

    public void CheckFlip()
    {
        if (Data.isFaceUp != CardVisuals.isFaceUp)
        {
            CardVisuals.Flip();
        }
    }

    public void Flip(bool flip = true)
    {
        if (flip != isFaceUp())
        {
            Data.isFaceUp = flip;
        }
    }
    public bool isFaceUp()
    {
        return Data.isFaceUp;
    }

    public void Shake()
    {
        CardVisuals.Shake();
    }

    public void Pulse()
    {
        CardVisuals.Pulse();
    }

    
}
