using UnityEngine;
using System.Collections;

public class InventoryPickup
{
    public InventoryPickup(string pName, ItemType pType, int pSerial)
    {
        itemName = pName;
        pickupType = pType;
        serial = pSerial;
    }

    public InventoryPickup(InventoryPickup ip)
    {
        itemName = ip.itemName;
        pickupType = ip.pickupType;
        serial = ip.serial;
    }

    public string itemName;

    public enum ItemType
    {
        tool,
        component,
        boost
    }

    public ItemType pickupType;

    public int serial;
}
