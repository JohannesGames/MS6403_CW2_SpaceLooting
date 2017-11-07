using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    //a sort of copy constructor
    //public Pickup(Pickup lastPickup)
    //{
    //    this.itemName = lastPickup.itemName;
    //    this.pickupType = lastPickup.pickupType;
    //}

    public string itemName;

    public enum ItemType
    {
        tool,
        component,
        boost
    }

    public ItemType pickupType;
}
