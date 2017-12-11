using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string itemName;

    public InventoryPickup.ItemType pickupType;

    [HideInInspector]
    public ParticleSystem particleSys;

    [HideInInspector]
    public int serial;

    public Sprite icon;

    void Start()
    {
        particleSys = GetComponentInChildren<ParticleSystem>();
    }
}
