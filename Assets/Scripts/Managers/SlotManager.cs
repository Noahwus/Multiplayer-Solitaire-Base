using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
//*
public class SlotManager : Singleton<SlotManager>
{
    public GameObject playerBoardPrefab;

    //Primary Collection
    public Dictionary<int,    PlayerSlot>   _playerSlots = new(); //<PlayerID, PlayerSlot(contains list of CardSlots)
    public Dictionary<string, CardSlot>     _cardSlots   = new(); //<SlotID, CardSlot>

    //Optimized Lookup
    public Dictionary<SlotType,List<CardSlot>>  _slotsByType   = new();
    public Dictionary<Card,       CardSlot>     _cardToSlotMap = new();
    public Dictionary<GameObject, CardSlot>     _objectToSlotMap = new();

    [Header("Distributing Game Objects")]
    [SerializeField] public int cardPaddingCount = 5;
    [SerializeField] public float cardPadding = .2f;
    [SerializeField] public float yOffGlobal = 0.01f;
    [SerializeField] public float maxPaddingDistance = 1.5f;

    [Header("Distributing PlayerBoards")]
    [SerializeField] private List<GameObject> playerBoardLocs = new List<GameObject>();
    public Vector3 GetPlayerBoardLocByIndex(int index) => playerBoardLocs[index].transform.position;

    public void SpawnPlayerBoard(int PlayerID, Vector3 spawnLoc)
    {
        PlayerSlot playerSlot         = new PlayerSlot();
        GameObject playerBoard        = Instantiate(playerBoardPrefab, spawnLoc, Quaternion.identity);
        SlotLayoutHelper layoutHelper = playerBoard.GetComponent<SlotLayoutHelper>();
        
        if (layoutHelper?.orderedSlots == null || layoutHelper.orderedSlots.Count == 0)
        {
            Debug.LogError("Invalid SlotLayoutHelper on player board prefab"); return;
        }

        int index = 0;
        foreach(CardSlot slot in layoutHelper.orderedSlots)
        {
            SlotType st = slot.Initialize(PlayerID, index);
            RegisterSlots(slot);
            
            index++;
        }

        playerSlot.Initialize(PlayerID, layoutHelper.orderedSlots);
        _playerSlots.Add(PlayerID, playerSlot);
        
    }

    public void RegisterSlots(CardSlot slot)
    {
        if (slot == null || slot.Data == null) return;

        SlotType type = slot.Data.slotType;

        if (!_slotsByType.TryGetValue(type, out var slots))
        {
            slots = new List<CardSlot>();
            _slotsByType[type] = slots;
        }

        if (!slots.Contains(slot)) // Prevent duplicates
        {
            slots.Add(slot);
        }

        string ID = slot.GetSlotID();
        if (!_cardSlots.ContainsKey(ID)) 
        {
            _cardSlots.Add(ID, slot);
        }

        _objectToSlotMap.Add(slot.gameObject, slot);
    }

    private void Awake()
    {
        NewInstance(this);
    }


    public void MoveCardAuthoritative(CardMoveData moveData)
    {
        Card card       = CardManager.Instance.GetCard(moveData.cardID);
        bool faceUp     = moveData.isFaceUp;
        CardSlot slot = SlotManager.Instance.GetSlotByID(moveData.targetSlot);

        MoveCardAuthoritative(card, slot);
    }

    public void MoveCardAuthoritative(Card card, CardSlot slot)
    {
        Debug.Log(Utils.ColorText("Moved Authroitatively", Color.white));
        CardSlot ogSlot = GetSlotByCard(card);

        RemoveCardFromSlot(card, ogSlot);
        AddCardToSlot(card, slot);

        card.Pulse();

        // Check Card flip (discard)
        if (slot.Data.GetSlotType() == SlotType.Discard )
        {
            card.Flip();
        }else if((slot.Data.GetSlotType() == SlotType.Deck && ogSlot.Data.GetSlotType() == SlotType.Discard))
        {
            card.Flip(false);
        }

        // Check Lane Flip
        CheckPlayLanesTopCardFlipped();
        // Check card flip (recycle)

        //Check Deck Empty
        CheckDeckEmpty();

        // fliped for any other reason?
    }

    public void AddCardToSlot( Card card, CardSlot slot)
    {
        if (slot.Data == null || card?.Data == null)
        {
            Debug.LogError("Null reference in AddCardToSlot");
            return;
        }

        string cardID       = card.Data.cardID;
        SlotData slotData   = slot.Data;

        slotData.AddCard(cardID);
        //Debug.Log($"Adding {cardID} to {slot.GetSlotID()}...");

        UpdateCardSlotMapping(card, slot);
    }

    private void UpdateCardSlotMapping(Card card, CardSlot slot)
    {
        if (slot == null)
            _cardToSlotMap.Remove(card);
        else
            _cardToSlotMap[card] = slot;
    }

    public void RemoveCardFromSlot(Card card, CardSlot slot)
    {
        if (slot == null || card?.Data == null) return;

        string cardID = card.Data.cardID;
        slot.Data.cardIDs.Remove(cardID);
        UpdateCardSlotMapping(card, null); // Clear mapping
    }


    public CardSlot GetSlotByCard(Card card)
    {
        if (card?.Data == null) return null;
        return _cardToSlotMap.TryGetValue(card, out var slot)
            ? slot
            : null;
    }

    public CardSlot GetSlotByObject(GameObject obj)
    {
        if(obj == null) return null;
        return _objectToSlotMap.TryGetValue(obj, out var slot) 
            ? slot 
            : null;
    }

    public CardSlot GetSlotByID(string slotID)
    {
        if (_cardSlots.TryGetValue(slotID, out CardSlot slot))
            return slot;

        Debug.LogWarning($"[SlotManager] Could not find slot with ID: {slotID}");

        string buffer = "Actual Slot IDs bozo:\n";
        foreach(var cardSlot in _cardSlots)
        {
            buffer += $"{cardSlot.Key}\n";
        }Debug.Log(buffer);
        return null;
    }

    public List<CardSlot> GetSlotsByType(SlotType slotType)
    {
        if (_slotsByType.TryGetValue(slotType, out List<CardSlot> cardSlots))
        {
            return cardSlots;
        }

        return new List<CardSlot>(); 
    }


    public void DebugPrintSlotContents()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Utils.ColorText("=== SLOT CONTENTS ===\n", Color.cyan));
        sb.AppendLine($"Total players: {_playerSlots.Count}");
        sb.AppendLine($"Total slots: {_slotsByType.Count}\n");
        // Loop through all players
        foreach (var playerEntry in _playerSlots) // Dictionary<int, Dictionary<int, SlotData>>
        {
            int playerId          = playerEntry.Key;
            PlayerSlot playerSlot = playerEntry.Value;
           
            sb.AppendLine(Utils.ColorText($"--- PLAYER [", Color.yellow)+$"{ Utils.ColorText(playerId.ToString(), Color.white)}" + $"{Utils.ColorText($"] SLOTS ---", Color.yellow)}");

            foreach(var slotEntry in playerSlot.CardSlots)
            {
                string slotId       = slotEntry.Key;
                CardSlot cardSlot   = slotEntry.Value;
                SlotData slotData   = cardSlot.Data;

                sb.AppendLine($"\t Slot[{slotData.slotIndex}]: {slotId} ({slotData.slotType.ToString()})");

                if (slotData.cardIDs.Count <= 0)
                {
                    sb.AppendLine($"\t\t (Empty)");
                }
                else
                {
                    for (int i = 0; i < slotData.cardIDs.Count; i++)
                    {
                        Card card = CardManager.Instance.GetCard(slotData.cardIDs[i]);
                        sb.AppendLine($"\t\t  [{i}] {(card != null ? ($"{card.Data.GetDebugName()} [FaceUp:{Utils.TF(card.isFaceUp())}]") : Utils.ColorText($"MISSING: {slotData.cardIDs[i]}", Color.red))}");

                    }
                }
            }


            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
        CardMoveManager.Instance.CardUpdate();
    }

    public List<CardSlot> GetPlayerSlotsByType(int playerId, SlotType type)
    {
        if (!_playerSlots.TryGetValue(playerId, out PlayerSlot playerSlot))
        {
            Debug.LogWarning($"Player {playerId} not found");
            return new List<CardSlot>();
        }

        return playerSlot.CardSlots.Values
            .Where(slot => slot.Data.slotType == type)
            .ToList();
    }


    public void ToggleCardSlotColliders(bool active)
    {
        foreach(var slot in _slotsByType)
        {
            SlotType type = slot.Key;
            foreach(var slotData in slot.Value)
            {
                if(type == SlotType.Deck) 
                    { slotData.Node.ToggleCollider(!slotData.HasCards()); }
                else 
                    { slotData.Node.ToggleCollider(active); }
            }
        }
    }

    public void CheckPlayLanesTopCardFlipped()
    {
        List<CardSlot> playSlots = GetSlotsByType(SlotType.Play);
        foreach (CardSlot cs in playSlots) 
        {
            Card card = cs.Data.GetTopCard();
            if (card == null) 
                { continue; }

            if (!card.isFaceUp())
                { card.Flip(true); }
        }

        List<CardSlot> slots = GetSlotsByType(SlotType.Score);
        foreach (CardSlot cs in playSlots)
        {
            Card card = cs.Data.GetTopCard();
            if (card == null)
                { continue; }

            if (!card.isFaceUp())
                { card.Flip(true); }
        }
    }

    public void CheckDeckEmpty()
    {
        List<CardSlot> deckSlots = GetSlotsByType(SlotType.Deck);
        foreach (CardSlot cs in deckSlots)
        {
            if (!cs.HasCards()) // No Cards? Turn on the clicker
            {
                Debug.Log("No cards left!");
                cs.Node.ToggleCollider(true);
            }
            else {  cs.Node.ToggleCollider(false); }    
        }
    }




    
}


