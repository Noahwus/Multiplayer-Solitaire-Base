using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomManager : MonoBehaviour
{
    //private static PhantomManager instance;

    [Header("Settings")]
    public int maxPhantoms = 5;
    public float childScaleReduction = .2f;
    public float childAlphaReduction = .15f;
    private Dictionary<NetworkConnection, List<PhantomCard>> _playerPhantoms = new();

    private void Awake()
    {
        //NewInstance(this);
    }

    public bool TryGetPhantom(NetworkConnection owner, out PhantomCard phantom)
    {
        phantom = null;
        if (_playerPhantoms.TryGetValue(owner, out var list) && list.Count < maxPhantoms)
        {
            //phantom = PhantomPool.Instance.Get();
            //phantom.Init(owner, list.Count);
            list.Add(phantom);
            return true;
        }
        return false;
    }
}
