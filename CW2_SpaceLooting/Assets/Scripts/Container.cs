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

    public List<GameObject> playersAccessing = new List<GameObject>();

    //[SyncVar]
    //public List<PCControl.ItemPickups> inContainer = new List<PCControl.ItemPickups>();

    void ContainerChanged(SyncListStruct<PCControl.ItemPickups>.Operation op, int itemIndex)
    {
        
    }

    [ClientRpc]
    public void RpcUpdateAllPlayers()
    {
        foreach (GameObject item in playersAccessing)
        {
            PCControl pc = item.GetComponent<PCControl>();
            pc.hM.InvokeUpdateContainer();
        }
    }

    void Start()
    {
        inContainer.Callback = ContainerChanged;
    }

    [ClientRpc]
    public void RpcAddPlayers(GameObject _pc)
    {
        for (int i = 0; i < playersAccessing.Count; i++)
        {
            if (_pc.GetInstanceID() == playersAccessing[i].GetInstanceID())
            {
                return;
            }
        }
        playersAccessing.Add(_pc);
    }

    [ClientRpc]
    public void RpcRemovePlayer(GameObject _pc)
    {
        for (int i = 0; i < playersAccessing.Count; i++)
        {
            if (_pc.GetInstanceID() == playersAccessing[i].GetInstanceID())
            {
                playersAccessing.RemoveAt(i);
            }
        }
    }


    //public void AddItemContainer(InventoryPickup tItem)
    //{
    //    if (isClient)
    //    {
    //        CmdAddItemOnClient(tItem.itemName, (int)tItem.pickupType, tItem.serial);
    //    }

    //    for (int i = 0; i < inContainer.Count; i++)
    //    {
    //        if (inContainer[i].serial == tItem.serial)
    //        {
    //            inContainer.Dirty(i);
    //        }
    //    }
    //}

    //[Command]
    //private void CmdAddItemOnClient(string _name, int _type, int _serial)
    //{
    //    PCControl.ItemPickups ip = new PCControl.ItemPickups(_name, _type, _serial);
    //    inContainer.Add(ip);
    //}

    //public void RemoveItemContainer(InventoryPickup tItem)
    //{
    //    PCControl.ItemPickups cp = new PCControl.ItemPickups(tItem);
    //    inContainer.Remove(cp);
    //}
}
