using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Container : NetworkBehaviour
{
    //public struct ContainerPickups
    //{
    //    public ContainerPickups(string _name, InventoryPickup.ItemType _type, int _serial)
    //    {
    //        itemName = _name;
    //        pickupType = _type;
    //        serial = _serial;
    //    }

    //    public ContainerPickups(InventoryPickup ip)
    //    {
    //        itemName = ip.itemName;
    //        pickupType = ip.pickupType;
    //        serial = ip.serial;
    //    }

    //    public string itemName;

    //    public InventoryPickup.ItemType pickupType;

    //    public int serial;
    //}

    public class SyncInContainer : SyncListStruct<HUDManager.ItemPickups>
    {
    }

    public SyncInContainer inContainer = new SyncInContainer();
    
    //public List<InventoryPickup> inContainer = new List<InventoryPickup>();

    void Start()
    {
        
    }
    
    void Update()
    {

    }

    public void AddItemContainer(InventoryPickup tItem)
    {
        inContainer.Add(new HUDManager.ItemPickups(tItem));
    }

    public void RemoveItemContainer(InventoryPickup tItem)
    {
        HUDManager.ItemPickups cp = new HUDManager.ItemPickups(tItem);
        inContainer.Remove(cp);
    }
}
