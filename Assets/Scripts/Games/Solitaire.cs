using FishNet.Demo.AdditiveScenes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Solitaire : CardGame
{
    private void Start()
    {
        StartGame();
    }
    private void Awake()
    {
        //NewInstance(this);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            StartCoroutine(UnShuffleSequence());
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            StartCoroutine(StartSequence());
        }
    }
    public override void StartGame()
    {
        List<int> playerIDs = new List<int>(); 
        playerIDs.Add(1);
        playerIDs.Add(2);
        playerIDs.Add(3);
        playerIDs.Add(4);

        base.StartGame();
        int i = 0;
        foreach (int PlayerId in playerIDs)
        {
            SlotManager.Instance.SpawnPlayerBoard(PlayerId, SlotManager.Instance.GetPlayerBoardLocByIndex(i));

            List<CardSlot> slots = SlotManager.Instance.GetPlayerSlotsByType(PlayerId, SlotType.Deck);
            List<Card> cards     = CardManager.Instance.SpawnDeckOfCards(PlayerId);

            foreach (Card card in cards)
            {
                SlotManager.Instance.AddCardToSlot(card, slots[0]);
            }
            slots[0].Data.Shuffle();

            i++;
        }

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        yield return StartCoroutine(CardMoveManager.Instance.CardUpdateRoutine(0.03f)); // Waits until A finishes
        SlotManager.Instance.DebugPrintSlotContents();
        
        yield return StartCoroutine(Deal()); // Then starts B

        yield return StartCoroutine(CardMoveManager.Instance.CardUpdateRoutine(0, .2f));
        SlotManager.Instance.DebugPrintSlotContents();
    }

    IEnumerator UnShuffleSequence()
    {
        yield return StartCoroutine(UnDeal()); // Waits until A finishes
        yield return StartCoroutine(CardMoveManager.Instance.CardUpdateRoutine(0.01f));
        SlotManager.Instance.DebugPrintSlotContents();

    }

    public override IEnumerator Deal()
    {
        foreach (var player in SlotManager.Instance._playerSlots)
        {
            int playerID = player.Key;
            PlayerSlot ps = player.Value;

            List<CardSlot> playSlots = SlotManager.Instance.GetPlayerSlotsByType(playerID, SlotType.Play);
            List<CardSlot> DeckSlot = SlotManager.Instance.GetPlayerSlotsByType(playerID, SlotType.Deck);
            CardSlot deck = DeckSlot[0];

            int CardsToDeal = 1;

            foreach(CardSlot targetSlot in playSlots)
            {
                for (int i = 0; i < CardsToDeal; i++) 
                {
                    if (deck.Data.cardIDs.Count == 0)
                    {
                        Debug.LogWarning("Deck ran out of cards during solitaire setup.");
                        yield break;
                    }

                    string cardID = deck.Data.GetTopCardID();
                    Card card     = CardManager.Instance._cards[cardID];

                    CardSlot ogSlot = SlotManager.Instance.GetSlotByCard(card);
                    CardSlot tarSlot = targetSlot;

                    SlotManager.Instance.MoveCardAuthoritative(card, tarSlot);

                    bool shouldBeFaceUp = (i == CardsToDeal - 1);
                    card.Flip(shouldBeFaceUp);
                }
                CardsToDeal++;
            }
        }

        yield return null;
    }

    public IEnumerator UnDeal()
    {
        foreach (var player in CardManager.Instance._playerCardList)
        {
            int playerID = player.Key;

            List<CardSlot> DeckSlot = SlotManager.Instance.GetPlayerSlotsByType(playerID, SlotType.Deck);
            CardSlot deck = DeckSlot[0];

            List<Card> cards = player.Value;

            foreach (Card card in cards) 
            {
                CardSlot ogSlot = SlotManager.Instance.GetSlotByCard(card);
                CardSlot tarSlot = deck;

                SlotManager.Instance.MoveCardAuthoritative(card, tarSlot);

                card.Flip(false);
            }
            deck.Data.Shuffle();
        }
        yield return null;
    }

    
    public static bool isValidMove(CardMoveData moveData)
    {
        int myPlayerID = 1; //Replace with FishNet player ID

        string cardID = moveData.cardID;
        string slotID = moveData.targetSlot;
        return isValidMove(cardID, slotID);
    }

    public static bool isValidMove(string cardID, string slotID)
    {
        int myPlayerID = 1; //Replace with FishNet player ID

        Card     card       = CardManager.Instance.GetCard(cardID);
        CardSlot targetSlot = SlotManager.Instance.GetSlotByID(slotID);

        //int indexOffset = 0;
        
        if(card == null || targetSlot == null) 
        { 
            if(card == null) { Debug.Log("CARD IS NULL"); }
            if(targetSlot == null) { Debug.Log("TARGETSLOT IS NULL"); }

            return false;
        }

        CardSlot currentSlot = SlotManager.Instance.GetSlotByCard(card);

        if (currentSlot == null) 
            { Debug.Log("Slot is Null"); return false; }

        if(currentSlot == targetSlot) 
            { Debug.Log($"{card.Data.GetCardID()} is already in {targetSlot.GetSlotID()}"); return false; }

       
        int currentSlotOwner     =  currentSlot.Data.ownerID;
        int targetSlotOwner      =  targetSlot.Data.ownerID;

        SlotType currentSlotType =  currentSlot.Data.GetSlotType();
        SlotType targetSlotType  =  targetSlot.Data.GetSlotType();
        
        if(!card.isFaceUp() && currentSlotType != SlotType.Deck) 
            { Debug.Log("Are you trying to use a power yupp?"); return false; }

        if (currentSlotType == SlotType.Deck && targetSlotType == SlotType.Discard) // Deck rotation easy breakout
        {
            if (currentSlotOwner != myPlayerID || targetSlotOwner != myPlayerID) 
                { Debug.Log("You don't own these king"); return false; }

            if (currentSlotOwner == targetSlotOwner) // If the owner is going through their own deck
                { Debug.Log("deck and disc");   return true; }
            else 
                { Debug.Log("not my deck");     return false; } // If it's not their deck!
        }
        else if(currentSlotType != SlotType.Deck && targetSlotType == SlotType.Discard)
            { Debug.Log($"Can't move card from {currentSlot.GetSlotID()} to Discard Slot"); return false; }


        if (currentSlotOwner != targetSlotOwner) // If the slot owners dont match...
        { 
            if(currentSlotOwner != myPlayerID)      // Current slot is not mine
            {
                if (currentSlotType == SlotType.Score) // Cannot Move Card off of Score Slot if it's not yours!
                { Debug.Log("cannot move off of score slot, not yours"); return false; }

                if (currentSlotType == SlotType.Deck) // Cannot Move Card off of deck Slot if it's not yours!
                { Debug.Log("cannot move off of deck slot, not yours");  return false; }
               
                if (!card.Data.isFaceUp) // Cannot move a Card that isn't yours and isn't Face Up
                { Debug.Log("cannot move if not face up and not yours"); return false; }

            }
            else                                    //If the target slot it not mine
            {
                if(targetSlotType == SlotType.Score) // This is you using your own card to score for another player?
                    { Debug.Log("Are you sure you want to score that for them?"); }

                if (targetSlotType == SlotType.Deck) // 
                    { Debug.Log("Yeah you cant move that to a Deck slot..."); return false; }

                if (targetSlotType == SlotType.Discard)
                    { Debug.Log("You can't aim for their discard slot?");     return false; }

            }
        }

        if(targetSlotType == SlotType.Deck && currentSlotType != SlotType.Discard) 
            { Debug.Log("Ain't it chief"); return false; }

        Card topCard = targetSlot.Data.GetTopCard();
        if (topCard == null)
            { Debug.Log("No Top Card..."); }

        if (targetSlotType == SlotType.Play)
        {
            if (topCard == null)
            {
                if ( card.Data.GetValue() != 13)
                    { Debug.Log("Kings can only go on Empty Playslot"); return false; }

                else if ( card.Data.GetValue() == 13)
                    { Debug.Log("Kings go here!"); return true; }
            }
            else { 
                if (!CardUtils.IsOppositeColor(card.Data, topCard.Data) && !topCard.Data.isWild() )
                    { Debug.Log("not opposite color"); return false; }

                if (!CardUtils.isNextValue(card.Data, topCard.Data))
                    { Debug.Log("not next value"); return false; }
            }
        }

        if (targetSlotType == SlotType.Score)
        {
            if (card.Data.GetValue() == 1 && targetSlot.Data.cardIDs.Count > 0) 
                { Debug.Log("Trying to put an Ace on an occupied scoreSlot"); return false; }
            else if(card.Data.GetValue() == 1 && targetSlot.Data.cardIDs.Count == 0) 
                { Debug.Log("Ace onto empty Slot!"); return true; }

            if (card.Data.GetSuit() != topCard.Data.GetSuit() && topCard != null)
                { Debug.Log("Need the same suit"); return false; }

            if (card.Data.GetValue() != topCard.Data.GetValue() + 1 && topCard != null)
                { Debug.Log("Needs to be the next card"); return false; }
        }

        if(targetSlot.Data.GetSlotType() == SlotType.Recycle && currentSlot.Data.ownerID != myPlayerID)
        {
            Debug.Log("Do not: Reduce, reuse, recycle!");
            return false;
        }

        Debug.Log("yeah king go ahead");
        return true; // No Break outs, so the move is valid.
    }

}


