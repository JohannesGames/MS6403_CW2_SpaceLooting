// ------ NW PC Anim Control ---------------
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class DD_NW_PC_Anim : NetworkBehaviour
{
    private Animator PC_Animator;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            PC_Animator = GetComponent<Animator>();            
        }
    }//-----


    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //---------------------------
        AnimatePC();
       
    }//-----

    private void AnimatePC()
    {

        /*
     // Arm trigger animation
     if (DD_PC_Movement.bl_PC_armed && !bl_armed)
     {
         PC_Animator.SetTrigger("arm");
         bl_armed = true;
     }
     else if (!DD_PC_Movement.bl_PC_armed && bl_armed)
     {
         PC_Animator.SetTrigger("unarm");
         bl_armed = false;
     }
     */

        // crouch
        if (DD_PC_Movement.bl_PC_crouching) PC_Animator.SetTrigger("crouch");
        else if (!DD_PC_Movement.bl_PC_crouching) PC_Animator.SetTrigger("stand");

        //  Jump 
        if (Input.GetButtonDown("Jump")) PC_Animator.SetBool("jump", true);
        else PC_Animator.SetBool("jump", false);

        //  Walking ---------------------------------------
        if (Input.GetAxis("Vertical") > 0) PC_Animator.SetBool("walkF", true);
        else PC_Animator.SetBool("walkF", false);

        if (Input.GetAxis("Vertical") < 0) PC_Animator.SetBool("walkB", true);
        else PC_Animator.SetBool("walkB", false);

        if (Input.GetAxis("Horizontal") > 0) PC_Animator.SetBool("walkR", true);
        else PC_Animator.SetBool("walkR", false);

        if (Input.GetAxis("Horizontal") < 0) PC_Animator.SetBool("walkL", true);
        else PC_Animator.SetBool("walkL", false);


        //  Run --------------------------------------------
        if (Input.GetKey(KeyCode.LeftShift)) PC_Animator.SetBool("run", true);
        else PC_Animator.SetBool("run", false);
    

    }//------

}//=========
