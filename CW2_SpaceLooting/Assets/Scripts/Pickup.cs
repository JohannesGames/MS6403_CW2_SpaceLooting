using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
    [SyncVar]
    public string itemName;

    [SyncVar]
    public int typeInt;
    public InventoryPickup.ItemType pickupType;
    public ParticleSystem particleSys;

    [HideInInspector]
    [SyncVar]
    public int serial;

    [HideInInspector]
    [SyncVar]
    public bool playParticle;

    private void Update()
    {
        if (playParticle && particleSys.isStopped)
        {
            particleSys.Play();
        }
        else if (!playParticle && particleSys.isPlaying)
        {
            particleSys.Stop();
        }

        switch (typeInt)
        {
            case 0:
                pickupType = InventoryPickup.ItemType.tool;
                break;
            case 1:
                pickupType = InventoryPickup.ItemType.component;
                break;
            case 2:
                pickupType = InventoryPickup.ItemType.boost;
                break;
            default:
                pickupType = InventoryPickup.ItemType.tool;
                break;
        }
    }
}
