// -------------------- NW PC Health ---------------
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DD_NW_Health : NetworkBehaviour
{
    // ----------------------------------------------------------------------
    public const int in_max_HP = 100;

    [SyncVar]
    public int in_HP = in_max_HP;
    
    private void Update()
    {
        // Display HP in the attached text mesh component
        GetComponentInChildren<TextMesh>().text = Mathf.Round(in_HP).ToString();

    }//------

    // ----------------------------------------------------------------------
    public void Damage(int _damage_amount)
    {
        if (!isServer)
            return;

        in_HP -= _damage_amount;
        if (in_HP <= 0)
        {

            in_HP = in_max_HP;
            // called on the server, will be invoked on the clients
            RpcRespawn();
        }
    }//-----

    // ----------------------------------------------------------------------

    private NetworkStartPosition[] spawnPoints;

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();

            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick a spawn point at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }//------

}//==========

