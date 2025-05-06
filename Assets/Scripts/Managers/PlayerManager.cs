using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonNetworkBehaviour<PlayerManager>
{
    public Dictionary<ulong, CustomNetworkObject> cats = new();

    public Dictionary<ulong, CustomNetworkObject> scoreUI = new();

    public Dictionary<int, ulong> ownerToSteamID = new();
    public Dictionary<ulong, int> steamToOwnerID = new();

    private OwnerSteamPair[] GetOwnerSteamPairs()
    {
        List<OwnerSteamPair> ownerSteamPairList = new List<OwnerSteamPair>();

        foreach (ulong steamID in steamToOwnerID.Keys)
        {
            ownerSteamPairList.Add(new OwnerSteamPair() { ownerId = steamToOwnerID[steamID], steamId = steamID });
        }

        return ownerSteamPairList.ToArray();
    }

    public void BroadcastOwnerPairs()
    {
        SetSteamPairs_Observer(GetOwnerSteamPairs());
    }

    [ObserversRpc(ExcludeServer = true)]
    public void SetSteamPairs_Observer(OwnerSteamPair[] ownerSteamPairs)
    {
        ownerToSteamID.Clear();

        steamToOwnerID.Clear();

        foreach (OwnerSteamPair ownerSteamPair in ownerSteamPairs)
        {
            ownerToSteamID.Add(ownerSteamPair.ownerId, ownerSteamPair.steamId);

            steamToOwnerID.Add(ownerSteamPair.steamId, ownerSteamPair.ownerId);
        }

        print("OWNER PAIRS RECEIVED");
    }

    private void Awake()
    {
        NewInstance(this);
    }
}
