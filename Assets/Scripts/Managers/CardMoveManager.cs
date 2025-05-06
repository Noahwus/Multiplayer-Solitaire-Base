using FishNet.Connection;
using FishNet.Object;
using Steamworks.Ugc;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// CardMoveManager manages where every card will go on "CardUpdate"
/// RequestMove() is our main Local function call to attempt a card move, before sending movedata to the Server Host
///     to validate
/// 
/// </summary>

public class CardMoveManager : SingletonNetworkBehaviour<CardMoveManager>
{
    private void Awake() => NewInstance(this);

    public Vector3 GetTargetCardPosition(Card card)
    {
        if (card == null) { Debug.Log("Card is null - GetTargetCardPosition()..."); return Vector3.zero; }

        CardSlot cs = SlotManager.Instance.GetSlotByCard(card);
        Vector3 offset = cs.Node.GetOffsetPosition(card.Data.GetCardID());

        return offset;
    }

    private Coroutine isUpdating;

    public Coroutine CardUpdate(float delay = 0, float altLerpTime = -1)
    {
        if (isUpdating != null) 
            { StopCoroutine(isUpdating); }

        isUpdating = StartCoroutine(CardUpdateRoutine(delay, altLerpTime));

        return isUpdating;
    }
    public IEnumerator CardUpdateRoutine(float delay = 0, float altLerpTime = -1)
    {
        
        foreach (var player in SlotManager.Instance._playerSlots)
        {
            int playerID = player.Key;
            PlayerSlot slot = player.Value;

            Dictionary<string, CardSlot> playerCardSlots = slot.CardSlots;

            foreach (var slotID in playerCardSlots)
            {
                string slotName = slotID.Key;
                CardSlot cs = slotID.Value;

                foreach (string cardID in cs.Data.cardIDs)
                {
                    Card card = CardManager.Instance.GetCard(cardID);

                    Vector3 targetPos = GetTargetCardPosition(card);

                    card.mover.EnumLerpToPosition(targetPos, altLerpTime);
                    card.CheckFlip();

                    if (delay > 0)
                    {
                        yield return new WaitForSeconds(delay);
                    }
                }
            }
        }

        isUpdating = null;
        yield return null;
    }

    // LOCAL Request for a card move
    public void RequestMove(Card card, CardSlot slot) // Overload 
    { 
        List<Card> cards = new();
        List<CardSlot> slots = new();

        if(card== null || slot == null) 
            { Debug.Log("card or slot was null..."); return; }

        cards.Add(card);
        slots.Add(slot);
        RequestMove(cards, slots);
    }

    // LOCAL Request for a card move
    public void RequestMove(List<Card> cards, List<CardSlot> slots) //Assume a list of cards/target slots so we can package multiple card movements
    {
        if (cards == null || slots == null)
            { Debug.LogError("Card or Slot array is null"); return;}


        if (cards.Count != slots.Count)
        {
            if (slots.Count == 1)
            {
                List<CardSlot> newSlots = new List<CardSlot>();
                foreach (Card card in cards)
                {
                    newSlots.Add(slots[0]);
                }
                slots.Clear();
                slots = newSlots;
            }
            else
            {
                Debug.LogError("There is a mismatch in card count / slot count, which cannot be fixed?");
                return;
            }
        }

        List<CardMoveData> data = new List<CardMoveData>();

        Debug.Log($"{cards[0].Data.cardID} to {slots[0].Data.slotID}");
        if (Solitaire.isValidMove(cards[0].Data.cardID, slots[0].GetSlotID())) // Check Move Valid Locally
        {
            for (int i = 0; i < cards.Count; i++)
            { 
                CardMoveData moved = new CardMoveData(
                cards[i].Data.cardID,
                slots[i].GetSlotID(),
                cards[i].isFaceUp()
                );
                data.Add(moved);
            }

            lastRequestedCard = cards[0];
            RequestMoveCard_ServerRPC(data.ToArray());
        }
        else
        {
            Debug.Log("Locally Denied...");
            HandleMoveRejected(cards[0].Data.GetCardID()); //Handle this locally if we catch early that this move ain't shit
        }
    }

    private Card lastRequestedCard;


    //The Following Network code is incomplete (FishNet [RPC]s are commented out for Local Offline playtesting) 

    // ServerRPC    (Client > Server | Runs on the Server)
    // ObserverRPC  (Server > all Clients | Runs on Clients)
    // TargetRPC    (Server > Specific Client | Runs on one Client)

    //[ServerRpc(RequireOwnership = false)] 
    private void RequestMoveCard_ServerRPC(CardMoveData[] moveData, NetworkConnection conn = null) //Client, run on Server to req move
    {
        if (moveData == null || moveData.Length == 0)
        {
            Debug.LogWarning("[Server] Recieved empty or Null MoveData array...");
            ReturnRequestResults_TargetRPC(conn, false, moveData);
            return;
        }
        Debug.Log($"{moveData[0].cardID} to {moveData[0].targetSlot}");
        bool valid = Solitaire.isValidMove(moveData[0].cardID, moveData[0].targetSlot); //Server is checking tha move is valid...
        if(valid)
        {
            ReceiveMoveCard_ObserverRPC(moveData);
            ReturnRequestResults_TargetRPC(conn, true, moveData); // Tell Client the move was valid via Positive Input Feedback
        }
        else
        {
            ReturnRequestResults_TargetRPC(conn, false, moveData); // Tell the client the move was invalid via Negative Input Feedback
            Debug.Log($"[Server] Invalid move for cardStack starting with : {moveData[0].cardID}");
        }
    }

    //[TargetRpc] //Returns feedback to the Card Move Requester 
    private void ReturnRequestResults_TargetRPC(NetworkConnection conn, bool success, CardMoveData[] moveData)
    {
        foreach (var move in moveData) 
        {
            if (!success)
            {
                Debug.Log("This Client is being told that move was not it chief.");
                HandleMoveRejected(move.cardID);
            }
            else
            {
                // Do Fun Feedback!!
            }
        }
    }

    private void HandleMoveRejected(string cardID)
    {
        Card tarCard = null;
        
        if (lastRequestedCard != null && lastRequestedCard.Data.GetCardID() == cardID)
        {
            tarCard = lastRequestedCard;
        }
        else
        {
            Card fallbackCard = CardManager.Instance.GetCard(cardID);
            tarCard = fallbackCard;
        }

        List<Card> temp = CardManager.Instance.GetCardStack(tarCard);
        if (temp.Count > 0)
        {
            temp.Reverse();
            foreach (Card card in temp)
                { card.Shake(); }
        }
        tarCard?.Shake();
        lastRequestedCard = null;
    }

    // This is the SERVER telling ALL CLIENTS to move the card.
    //[ObserversRpc]
    private void ReceiveMoveCard_ObserverRPC(CardMoveData[] moveData)
    {
        foreach (var move in moveData) 
        {
            // Actually perform the card move locally on this client.
            HandleCardMove(move);
        }
        CardUpdate(0, 0.05f);
    }

    private void HandleCardMove(CardMoveData moveData)
    {
        SlotManager.Instance.MoveCardAuthoritative(moveData);
    }
}
