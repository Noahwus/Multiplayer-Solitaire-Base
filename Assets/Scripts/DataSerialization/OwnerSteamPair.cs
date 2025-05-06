using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OwnerSteamPair
{
    public int ownerId;
    public ulong steamId;

    public OwnerSteamPair(int ownerId, ulong steamId)
    {
        this.ownerId = ownerId;
        this.steamId = steamId;
    }
}



