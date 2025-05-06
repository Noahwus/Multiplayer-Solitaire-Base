using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomCard : MonoBehaviour
{
    [SerializeField] private CardVisual cardVisual;
    private Material _material;
    private int _phantomIndex = 0; // 0-primary, +1 per child

    public void Initialize(NetworkConnection owner, int index)
    {

    }
}
