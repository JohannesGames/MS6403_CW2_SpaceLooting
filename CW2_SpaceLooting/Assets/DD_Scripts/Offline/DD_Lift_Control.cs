// ----------------------------------------------------------------------
// -------------------- 3D Lift Animation Control 
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_Lift_Control : MonoBehaviour
{
    // ----------------------------------------------------------------------
    private Animator an_lift;
    private GameObject go_trigger_object;
    public float fl_distance = 5;

    // ----------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        an_lift = GetComponent<Animator>();
        go_trigger_object = GameObject.Find("PC");
    }//-----

    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {     
        // In trigger distance
        if (Vector3.Distance(go_trigger_object.transform.position, transform.position) < fl_distance)
        {         
            // Change lft state
            if (Input.GetKeyDown("e"))
            {             
                if (an_lift.GetBool("bl_lift_up"))
                    an_lift.SetBool("bl_lift_up", false);
                else
                    an_lift.SetBool("bl_lift_up", true);                  
             }
        }
    }//------

}//==========
