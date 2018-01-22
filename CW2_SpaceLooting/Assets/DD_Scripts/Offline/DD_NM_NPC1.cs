// ----------------------------------------------------------------------
// -------------------- 3D NPC Navmesh
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DD_NM_NPC1 : MonoBehaviour
{
    // ----------------------------------------------------------------------
    public Transform tx_target;
    private Vector3 v3_destination;
    private NavMeshAgent nm_agent;

    // ----------------------------------------------------------------------
    void Start()
    {
        nm_agent = GetComponent<NavMeshAgent>();
        v3_destination = tx_target.position;
    }//-----


    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        FindTarget();

    }//-----

    // ----------------------------------------------------------------------
    void FindTarget()
    {
        if (Vector3.Distance(tx_target.position, v3_destination) > 1)
        {
            v3_destination = tx_target.position;
            nm_agent.destination = v3_destination;
        }
    }//-----

}//========
