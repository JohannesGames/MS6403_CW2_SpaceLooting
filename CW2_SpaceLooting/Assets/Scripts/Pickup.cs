using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [HideInInspector]
    public int inventoryIndex = -1;

    public string itemName;

    public enum ItemType
    {
        tool,
        component,
        boost
    }

    public ItemType pickupType;

    void Start()
    {

    }


    void Update()
    {

    }
}
