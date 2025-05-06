using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonNetworkBehaviour<T>: NetworkBehaviour where T : NetworkBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError($"[NetworkBehaviourSingleton] {typeof(T)} is NULL. Make sure you called NewInstance in Awake or OnStart methods.");
            return instance;
        }
    }

    protected virtual void NewInstance(T obj)
    {
        instance = obj;
    }
}
