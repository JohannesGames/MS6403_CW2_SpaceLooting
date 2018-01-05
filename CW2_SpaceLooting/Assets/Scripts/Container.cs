using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Container : NetworkBehaviour
{

    //public class SyncInContainer : SyncListStruct<LayoutManager.ItemPickups>
    //{
    //}

    //public SyncInContainer inContainer = new SyncInContainer();

    public static int maxContainerSize = 10;

    [SyncVar]
    public int[] inContainer = new int[maxContainerSize];

    public bool AddItemContainer(int pSerial)
    {
        int nextIndex = FindNextEmpty;  // find next available slot in array
        if (nextIndex <= 0 && !CheckIfRepeat(pSerial))
        {
            CmdAddItemContainer(pSerial, nextIndex);
            return true;
        }
        else
        {
            print("No available spaces in container");
            return false;
        }
    }

    [Command]
    private void CmdAddItemContainer(int puSerial, int nIndex)
    {
            inContainer[nIndex] = puSerial;
    }

    [ClientRpc]
    public void RpcAddItemContainer(InventoryPickup tItem)
    {
        //inContainer.Add(new PCControl.ItemPickups(tItem));
    }

    public void RemoveItemContainer(InventoryPickup tItem)
    {
        LayoutManager.ItemPickups cp = new LayoutManager.ItemPickups(tItem);
        //inContainer.Remove(cp);
    }

    public bool CheckIfRepeat(int tSerial)
    {
        for (int i = 0; i < inContainer.Length; i++)
        {
            if (tSerial == inContainer[i])
            {
                return true;
            }
        }
        return false;
    }

    public int FindNextEmpty
    {
        get
        {
            for (int i = 0; i < inContainer.Length; i++)
            {
                if (inContainer[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}