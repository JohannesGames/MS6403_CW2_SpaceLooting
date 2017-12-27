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

    [Command]
    public void CmdAddItemContainer(InventoryPickup tItem)
    {
        RpcAddItemContainer(tItem);
    }

    [ClientRpc]
    public void RpcAddItemContainer(InventoryPickup tItem)
    {
        inContainer.Add(new PCControl.ItemPickups(tItem));
    }

    public void RemoveItemContainer(InventoryPickup tItem)
    {
        PCControl.ItemPickups cp = new PCControl.ItemPickups(tItem);
        inContainer.Remove(cp);
    }
}
