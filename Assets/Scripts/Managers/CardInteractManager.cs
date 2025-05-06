using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class CardInteractManager : Singleton<CardInteractManager>
{
    private void Awake() => NewInstance(this);

    private float mouseDownTime;
    private Card selectedCard;
    private CardSlot selectedSlot;
    private bool isDragging = false;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            mouseDownTime = Time.time;

            GameObject obj = Utils.IsMouseOverObject();
            Debug.Log(obj.name + " clicked on...");

            Card card = Utils.IsMouseOverCard();
            if(card != null)
            {
                selectedCard = card;
            }
            else { Debug.Log($"Utils returned a{Utils.ColorText(" null ", Color.red)} card to Interaction Manager"); }

            CardSlot cs = SlotManager.Instance.GetSlotByObject(obj.transform.parent.gameObject);
            if (cs != null)
            {
                selectedSlot = cs;
                Debug.Log(cs.GetSlotID());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                Debug.Log("TRYING TO END THE DRAG");
                if (selectedCard != null)
                { TryCardEndDrag(); }
            }
            else
            {
                //Debug.Log("TRYING TO CLICK");

                if (selectedCard != null)
                { TryCardClick(selectedCard); }

                if (selectedSlot != null)
                { TrySlotClick(selectedSlot); }
            }
        }

        float holdDuration = -1;
        if (Input.GetMouseButton(0)) 
            { holdDuration = Time.time - mouseDownTime;}
        
        if (holdDuration > .15f && !isDragging && selectedCard != null) 
            { TryCardDrag(selectedCard); }

        if (Input.GetKeyDown(KeyCode.Space))
            { SlotManager.Instance.DebugPrintSlotContents(); }
    }

    public void TryCardClick(Card card)
    {
        CardSlot slot = SlotManager.Instance.GetSlotByCard(card);

        if(slot != null) 
        { 
            if(slot.Data.slotType == SlotType.Deck)
            {
                List<CardSlot> tarSlot = SlotManager.Instance.GetPlayerSlotsByType(slot.Data.ownerID, SlotType.Discard);
                CardMoveManager.Instance.RequestMove(card, tarSlot[0]);
                //Debug.Log("TRYNA QUICK CLICK THIS SHIT");

            }
            if(slot.Data.slotType == SlotType.Score) { Debug.Log(""); }
            else
            {
                List<CardSlot> potentialMoves = CardManager.Instance.GetViableMoves(card);
                string buffer = $"Click - Potential Moves for({card.Data.GetDebugName()}): \n";

                foreach(CardSlot potentialMove in potentialMoves)
                {
                    string target = null;
                    if (potentialMove.Data.HasCards())
                        { target = potentialMove.Data.GetTopCard().Data.GetDebugName() + $" in {potentialMove.GetSlotID()}"; }
                    else 
                        { target = potentialMove.GetSlotID(); }

                    buffer +="\t" + target + "\n";
                }

                Debug.Log(buffer);

            }
        }
        else { Debug.Log("CardSlot slot was null for 'GetSlotByCard'"); }
        
        EndInteractions();
    }

    public void TrySlotClick(CardSlot slot)
    {   
        int pID = slot.Data.ownerID;
        if(slot.Data.slotType == SlotType.Deck)
        {
            if (slot.HasCards())
            { }
           // SlotManager.Instance.CheckDeckEmpty(); return; }

            CardSlot fromSlot = SlotManager.Instance.GetPlayerSlotsByType(pID, SlotType.Discard)[0]; //Get The Discard
                if (fromSlot == null) { return; }

            Card card = fromSlot.GetTopCard(); //Make sure the discard isn't empty
                if (card == null) { return; }

            List<Card> cards = fromSlot.Data.GetAllCardsCards();
                cards.Reverse();

            List<CardSlot> slots = new(); 
                slots.Add(slot);

            CardMoveManager.Instance.RequestMove(cards, slots);
        }
    }

    public void TryCardDrag(Card card)
    {
        Debug.Log($"Try Drag {card.Data.GetDebugName()}");
        if (!isDragging)
        {
            isDragging = true;
            SlotManager.Instance.ToggleCardSlotColliders(true);
        }
    }

    public void TryCardEndDrag()
    {
        GameObject obj = Utils.IsMouseOverObject();
       
        List<Card> cardMoves = new List<Card>();
        List<CardSlot> slots = new List<CardSlot>();

        if (selectedCard == null) 
            { return; }

        if (obj == null || obj.transform.parent == null)
            { return; }


        CardSlot cs = SlotManager.Instance.GetSlotByObject(obj.transform.parent.gameObject);
        if (cs == null)
            { return; }


        List<Card> cardStack = new List<Card>();
        if (selectedCard.isBurried())
            { cardStack = CardManager.Instance.GetCardStack(selectedCard); }


        cardMoves.Add(selectedCard);
        slots.Add(cs);

        if (cardStack.Count > 0)
        {
            foreach (Card card in cardStack)
            {
                cardMoves.Add(card);
                slots.Add(cs);
            }
        }

        string buffer = $"Attempt to move:\n ";
        int i = 0;
        foreach (Card card in cardMoves)
        {
            buffer += $"\t{cardMoves[i].Data.GetDebugName()} to {slots[i].GetSlotID()}\n";
            i++;
        }
        Debug.Log(buffer);
      
        CardMoveManager.Instance.RequestMove(cardMoves, slots);
        EndInteractions();
    }

    public void EndInteractions()
    {
        //Debug.Log("ENDING THE INTERAACTION");
        //mouseDownTime   = -99999;
        isDragging      = false;
        selectedCard    = null;
        selectedSlot    = null;
        SlotManager.Instance.ToggleCardSlotColliders(false);
    }


}
