using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class CardManager : Singleton<CardManager> 
{
    // Variables and References
    public GameObject cardPrefab;
    public GameObject cardParent;

    public Dictionary<string,     Card>     _cards = new();
    public Dictionary<GameObject, Card>   _idCards = new();
    public Dictionary<int, List<Card>>    _playerCardList = new();

    List<CardTemplate> cardTemplates;

    [SerializeField]
    public List<CardModifier> modifiers = new List<CardModifier>();

    private void Awake() 
    { 
        NewInstance(this); 
        cardTemplates = CardFactory.CreateStandardCardTemplates(); 
    }



    // Getters
    public Card GetCard(string id)                   => _cards[id];
    public GameObject GetCardObjectByID(string id)   => _cards[id].gameObject;
    


    // Functions
    public List<Card> SpawnDeckOfCards(int playerID)
    {
        List<CardTemplate> templates = cardTemplates;
        List<CardInstance> instances = CardFactory.CreateDeckFromTemplates(templates, playerID);

        List<Card> cards = new List<Card>();

        foreach (CardInstance cardInstance in instances)
        {
            // Spawn each card with playerID for ownership
            GameObject cardGO = SpawnCard(cardInstance, playerID);
                       cardGO.transform.SetParent(cardParent.transform);

            Card card = cardGO.GetComponent<Card>();

            Material mat = FaceMaterialManager.Instance.GetFaceMat(cardInstance);

            card.CardVisuals.SetCardFace(mat);
            cards.Add(card);
        }

        _playerCardList.Add(playerID, cards);
        return cards;
    }

    public GameObject SpawnCard(CardInstance cardInstance, int playerID)
    {
        // Instantiate the card prefab at the desired location
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity);

        // Initialize the card and get the card ID
        string cardID = cardGO.GetComponent<Card>().Initialize(cardInstance);

        cardGO.name = cardID;

        // Assign ownership to the card
        Card card = cardGO.GetComponent<Card>();
        card.AssignOwnership(playerID);

        _cards.Add(cardID, card);
        _idCards.Add(cardGO, card);

        return cardGO;
    }

    // Card Look ups! \\
    public List<CardSlot> GetViableMoves(Card myCard)
    {
        int myPlayerID = 1;
        List<Card> cards = new List<Card>();
        List<CardSlot> slots = new List<CardSlot>();

        //play slots
        int myTargetPlayVal           = myCard.Data.GetValue() + 1; //Value above this card's for Play lanes
        int myTargetScoreVal          = myCard.Data.GetValue() - 1; //Value below this card's for Score lanes
        CardColor myCurrentColor      = myCard.Data.GetColor();
        CardSuit  myCurrentSuit       = myCard.Data.GetSuit();

        foreach(var slot in SlotManager.Instance._cardSlots)
        {
            CardSlot cs = slot.Value;
            int ownerID = cs.Data.ownerID;
            
            SlotType st = cs.Data.GetSlotType();
            if (st == SlotType.Discard || st == SlotType.Deck) { continue; }
            if (st == SlotType.Score && ownerID != myPlayerID) { continue; }


            if (myTargetScoreVal == 0 && st == SlotType.Score) // If ACE and ScoreSlot is empty
            {
                if (!cs.Data.HasCards())
                { slots.Add(cs); }
            }

            if(myTargetPlayVal == 14 && st == SlotType.Play) // If KING and PlaySlot is empty
            {
                if (!cs.Data.HasCards()) 
                    { slots.Add(cs); }
            }

            Card pCard  = cs.GetTopCard(); // Potential card

            if(pCard == null) 
                { continue; } 
            int currVal = pCard.Data.GetValue();

            if (currVal != myTargetPlayVal)  // If it don't equal this...
            {
                if (currVal != myTargetScoreVal) // or this... Just keep going
                { continue; } 
            }
            
            if(currVal == myTargetPlayVal && st == SlotType.Play) //
            {
                CardColor currColor = pCard.Data.GetColor(); 

                if (currColor != myCurrentColor) 
                    { cards.Add(pCard); slots.Add(cs); }

            }
            else if (currVal == myTargetScoreVal && st == SlotType.Score)
            {
                CardSuit currSuit = pCard.Data.GetSuit();

                if(currSuit == myCurrentSuit) 
                    { cards.Add(pCard); slots.Add(cs); }
            }

        }

        //No longer using cards?
        return slots;
    }

    public List<Card> GetCardsByPlayerID(int playerID)
    {
        if (_playerCardList.TryGetValue(playerID, out var cards))
        {
            return cards;
        }

        return new List<Card>(); // Return an empty list if the ID isn't found
    }

    public List<Card> GetCardStack(Card parentCard)
    {
        List<Card> cardStack = new List<Card>();

        if(parentCard == null) 
            { return cardStack; }

        if (!parentCard.isBurried())
            { return cardStack; }

        CardSlot cs     = SlotManager.Instance.GetSlotByCard(parentCard);
        int indexInSlot = cs.Data.GetCardIndex(parentCard.Data.GetCardID());


        if (indexInSlot == -1 || indexInSlot >= cs.Data.cardIDs.Count - 1)
            return cardStack; // Either card isn't found or is already on top


        List<string> buriedCardIDs = cs.Data.cardIDs.Skip(indexInSlot + 1).ToList();  // Skip the first card which is on top
        return buriedCardIDs
            .Select(id => CardManager.Instance.GetCard(id))
            .Where(card => card != null)
            .ToList();
    }

}
