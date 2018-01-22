// ----------------------------------------------------------------------
// -------------------- 3D PC Animation - Dynamics
// -------------------- David Dorrington, UEL Games, 2016
// ----------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class DD_PC_Animation_Control : MonoBehaviour {

    private Animator PC_Animator;
    private bool bl_armed;

    // Use this for initialization
    void Start () {
        PC_Animator = GameObject.Find("PC_Mesh").GetComponent<Animator>();
        
    }//-----
	
	// Update is called once per frame
	void Update () {
        AnimatePC();
    }//-----


    //-------------------------------------------------------------------------
    // Animate PC
    void AnimatePC()
    {
        // reset the mesh position
        GameObject.Find("PC_Mesh").transform.localPosition = new Vector3(0, -1, 0);
              

        // Arm trigger animation
        if (DD_PC_Movement.bl_PC_armed && !bl_armed)
        {
            PC_Animator.SetTrigger("arm");
            bl_armed = true;
        }
       else  if (!DD_PC_Movement.bl_PC_armed && bl_armed)
        {
            PC_Animator.SetTrigger("unarm");
            bl_armed = false;
        }


        // crouch
        if (DD_PC_Movement.bl_PC_crouching) PC_Animator.SetTrigger("crouch");
        else if (!DD_PC_Movement.bl_PC_crouching) PC_Animator.SetTrigger("stand");
        


        // Standing ----------------------------------------


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


    }//-----


}//-==========
