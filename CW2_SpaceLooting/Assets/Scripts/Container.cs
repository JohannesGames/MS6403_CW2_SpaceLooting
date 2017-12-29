using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Container : NetworkBehaviour
{

    //public class SyncInContainer : SyncListStruct<PCControl.ItemPickups>
    //{
    //}

    //public SyncInContainer inContainer = new SyncInContainer();

    public static int maxContainerSize = 10;

    [SyncVar]
    public PCControl.ItemPickups[] inContainer = new PCControl.ItemPickups[10];

    [Command]
    public void CmdAddItemContainer(InventoryPickup tItem)
    {
        //RpcAddItemContainer(tItem);
        //inContainer.Add(new PCControl.ItemPickups(tItem));
    }

    [ClientRpc]
    public void RpcAddItemContainer(InventoryPickup tItem)
    {
        //inContainer.Add(new PCControl.ItemPickups(tItem));
    }

    public void RemoveItemContainer(InventoryPickup tItem)
    {
        PCControl.ItemPickups cp = new PCControl.ItemPickups(tItem);
        //inContainer.Remove(cp);
    }

    public int FindNextEmpty
    {
        get
        {
            for (int i = 0; i < inContainer.Length; i++)
            {
                if (inContainer[i].itemName == null)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}