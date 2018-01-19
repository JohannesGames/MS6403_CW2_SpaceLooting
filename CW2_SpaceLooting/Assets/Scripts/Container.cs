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

    public ParticleSystem containerParticle;

    //[SyncVar]
    //public List<PCControl.ItemPickups> inContainer = new List<PCControl.ItemPickups>();

    void ContainerChanged(SyncListStruct<PCControl.ItemPickups>.Operation op, int itemIndex)    //TODO decide whether this is necessary
    {
        
    }

    [ClientRpc]
    public void RpcUpdateAllPlayers()   // TODO should this be in the Callback?
    {
        foreach (GameObject item in playersAccessing)
        {
            PCControl pc = item.GetComponent<PCControl>();
            pc.hM.InvokeUpdateContainer();
        }
    }

    void Start()
    {
        //inContainer.Callback = ContainerChanged;
    }

    [ClientRpc]
    public void RpcAddPlayers(GameObject _pc)
    {
        containerParticle.Stop();
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
}
