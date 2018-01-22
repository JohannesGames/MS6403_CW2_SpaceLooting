// ----------------------------------------------------------------------
// -------------------- 3D Move
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DD_PC_Move : MonoBehaviour
{

    //-------------------------------------------------------------------------
    // ----- Movement Variables
    public float fl_speed = 6.0F;
    public float fl_jump_force = 8.0F;
    public float fl_gravity = 20.0F;
    public float fl_mouse_rotation_rate = 180;
    private Vector3 v3_move_direction = Vector3.zero;
    private CharacterController cc_PC; 


    //---------------------------------------------
    // Use this for initialization
    void Start()
    {
        // get a reference to the attached Character Controller
        cc_PC = GetComponent<CharacterController>();               

    }//-----

    //-------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        MovePC();

    }//-----


    //-------------------------------------------------------------------------
    //  PC Movement control
    void MovePC()
    {
        //  PC Ground Movement
        if (cc_PC.isGrounded)
        {
            // Add X & Z movement to the direction vector based input axes (W,A,S,D or Cursor) 
            v3_move_direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Convert world coordinates to local for the PC and multiply by speed
            v3_move_direction = fl_speed * transform.TransformDirection(v3_move_direction);

            // if the jump key is pressed add jump force to the direction vector
            if (Input.GetButton("Jump")) v3_move_direction.y = fl_jump_force;
        }

        // Add fl_gravity to the direction vector
        v3_move_direction.y -= fl_gravity * Time.deltaTime;

        // Move the character controller with the direction vector
        cc_PC.Move(v3_move_direction * Time.deltaTime);
    }// ----- 

}//==========
