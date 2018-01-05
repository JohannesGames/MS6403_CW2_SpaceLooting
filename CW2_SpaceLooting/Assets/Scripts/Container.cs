using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Container : NetworkBehaviour
{

    public class SyncInContainer : SyncListStruct<PCControl.ItemPickups>
    {
    }

    public SyncInContainer inContainer = new SyncInContainer();

    //[SyncVar]
    //public List<PCControl.ItemPickups> inContainer = new List<PCControl.ItemPickups>();

    void ContainerChanged(SyncListStruct<PCControl.ItemPickups>.Operation op, int itemIndex)
    {
        Debug.Log("list changed:" + itemIndex);
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            return;
        }
        inContainer.Callback = ContainerChanged;
    }


    public void AddItemContainer(InventoryPickup tItem)
    {
        inContainer.Add(new PCControl.ItemPickups(tItem));
    }

    public void RemoveItemContainer(InventoryPickup tItem)
    {
        PCControl.ItemPickups cp = new PCControl.ItemPickups(tItem);
        inContainer.Remove(cp);
    }
}
