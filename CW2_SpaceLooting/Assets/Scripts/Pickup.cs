using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
    [SyncVar]
    public string itemName;

    public InventoryPickup.ItemType pickupType;
    public ParticleSystem particleSys;

    [HideInInspector]
    [SyncVar]
    public int serial;
}
