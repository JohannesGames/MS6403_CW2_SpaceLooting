using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
    [SyncVar]
    public string itemName;

    public InventoryPickup.ItemType pickupType;

    [HideInInspector]
    public ParticleSystem particleSys;

    [HideInInspector]
    [SyncVar]
    public int serial;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        particleSys = GetComponentInChildren<ParticleSystem>();
    }
}
