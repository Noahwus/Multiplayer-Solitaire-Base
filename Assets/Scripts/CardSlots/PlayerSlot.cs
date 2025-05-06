using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerSlot 
{
    public int PlayerID {  get; private set; }
    public Dictionary<string, CardSlot> CardSlots;

    public void Initialize(int ID, List<CardSlot> slots)
    {
        PlayerID = ID;
        Dictionary<string, CardSlot> newDict = new Dictionary<string, CardSlot>();

        foreach (CardSlot slot in slots)
        {
            if(slot == null) { Debug.Log("slots pass in are Null!"); return; }

            string id = slot.GetSlotID();
            newDict.Add(id, slot);
        }      
        
        CardSlots = newDict;
    }

    public struct NetworkData
    {
        public int      playerID;
        public string[] slotIDs; // CardSlot.GetSlotID()
        public int[]    slotIndices;
    }
}
