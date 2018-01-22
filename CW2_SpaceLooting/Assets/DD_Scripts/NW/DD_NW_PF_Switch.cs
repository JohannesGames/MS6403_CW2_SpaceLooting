// ----------------------------------------------------------------------
// -------------------- 3D Platform Anim Switch Control
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DD_NW_PF_Switch : NetworkBehaviour
{

    // ----------------------------------------------------------------------
    private Animator an_platform;
    private GameObject go_trigger_object;
    public float fl_distance = 2;
    public int in_stage = 1;
    private DD_NW_Platfrom_Control mSCR_PlatformControl;

    // ----------------------------------------------------------------------
    // Update is called once per frame

    void Update()
    {
        if (!go_trigger_object)
        {
            FindLocalPC();
        }
        else
        {
            ActivateSwitch();
        }
    }//-----


    // ----------------------------------------------------------------------
    void ActivateSwitch()
    {
        if (!an_platform) an_platform = GameObject.Find("anim_platform").GetComponent<Animator>();

        //// In trigger distance
        if (Vector3.Distance(go_trigger_object.transform.position, transform.position) < fl_distance)
        {
            GetComponent<Renderer>().material.color = Color.red;
            // activate switch
            if (Input.GetKeyDown("e"))
            {              
                GetComponent<Renderer>().material.color = Color.green;
               
            }
        }
    }//----



    // ----------------------------------------------------------------------
    void FindLocalPC()
    {
        GameObject[] gos_players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject _go in gos_players)
        {
            if (_go.GetComponent<NetworkIdentity>().isLocalPlayer)
                go_trigger_object = _go;
        }

    }//-----

}//================
