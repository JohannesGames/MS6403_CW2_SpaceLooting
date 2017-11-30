using UnityEngine;
using System.Collections;

public class InventoryPickup
{
    public InventoryPickup()
    {}

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

    public InventoryPickup(Pickup pu)
    {
        itemName = pu.itemName;
        pickupType = pu.pickupType;
        serial = pu.serial;
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
