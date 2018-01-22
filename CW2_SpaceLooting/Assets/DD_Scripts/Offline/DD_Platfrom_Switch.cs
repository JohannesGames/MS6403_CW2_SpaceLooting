// ----------------------------------------------------------------------
// -------------------- 3D Platfrom Switch 
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DD_Platfrom_Switch : MonoBehaviour
{
    // ----------------------------------------------------------------------
    public string st_switch = "bl_stage_01";


    private Animator an_platform;
    private GameObject go_trigger_object;
    public float fl_distance = 2;

    // ----------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
       
    }//-----

    // ----------------------------------------------------------------------
    // Update is called once per frame
 
    void Update()
    {      
     
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
                an_platform.SetBool(st_switch, true);
                GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }//----


  


}//==========
