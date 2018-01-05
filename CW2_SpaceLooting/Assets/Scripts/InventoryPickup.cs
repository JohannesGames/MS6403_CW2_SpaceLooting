using UnityEngine;
using System.Collections;

public class InventoryPickup
{
    public InventoryPickup()
    { }

    #region Constructors

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

    public InventoryPickup(LayoutManager.ItemPickups ip)
    {
        itemName = ip.itemName;
        pickupType = (ItemType)ip.pickupType;
        serial = ip.serial;
    }
    #endregion

    public string itemName;

    public enum ItemType
    {
        tool = 0,
        component = 1,
        boost = 2
    }

    public ItemType pickupType;

    public int serial;
}
