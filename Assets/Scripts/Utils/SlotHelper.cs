using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHelper : MonoBehaviour
{
    [SerializeField] private GameObject loc1;
    [SerializeField] private GameObject loc2;
    
    public (Vector3 pos1, Vector3 pos2) GetSlotLocs()
    {
        return (loc1.transform.position, loc2.transform.position);
    }
}
