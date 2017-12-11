using UnityEngine;
using System.Collections;

public class InventoryPickup
{
    public InventoryPickup()
    { }

    #region Constructors

    public InventoryPickup(string pName, ItemType pType, int pSerial, Sprite pIcon)
    {
        itemName = pName;
        pickupType = pType;
        serial = pSerial;
        icon = pIcon;
    }

    public InventoryPickup(string pName, ItemType pType, int pSerial)
    {
        itemName = pName;
        pickupType = pType;
        serial = pSerial;

        switch (pType)
        {
            case ItemType.tool:
                icon = LocalGameManager.LGM.toolIcon;
                break;
            case ItemType.component:
                icon = LocalGameManager.LGM.compIcon;
                break;
            case ItemType.boost:
                icon = LocalGameManager.LGM.boostIcon;
                break;
        }
    }

    public InventoryPickup(InventoryPickup ip)
    {
        itemName = ip.itemName;
        pickupType = ip.pickupType;
        serial = ip.serial;
        icon = ip.icon;
    }

    public InventoryPickup(Pickup pu)
    {
        itemName = pu.itemName;
        pickupType = pu.pickupType;
        serial = pu.serial;
        icon = pu.icon;
    }
    #endregion

    public string itemName;

    public enum ItemType
    {
        tool,
        component,
        boost
    }

    public ItemType pickupType;

    public int serial;

    public Sprite icon;
}
