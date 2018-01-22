using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_3D_PC_Move : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // ----- Movement Variables
    public float fl_speed = 6.0F;
    public float fl_jump_force = 8.0F;
    public float fl_gravity = 20.0F;
    private Vector3 V3_move_direction = Vector3.zero;
    private float fl_initial_speed;
    private CharacterController CC_PC;

    //-------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        // Find the attached Character Controller
        CC_PC = GetComponent<CharacterController>();
        fl_initial_speed = fl_speed;
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
               
        // If the run key is down double the speed
        if (Input.GetKey(KeyCode.LeftShift)) 
            fl_speed = fl_initial_speed * 2;
        else 
            fl_speed = fl_initial_speed;
        
        //  PC Ground Movement
        if (CC_PC.isGrounded)
        {
            // Add X & Z movement to the direction vector based input axes (W,A,S,D or Cursor) 
            V3_move_direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Convert world coordinates to local for the PC and multiply by speed
            V3_move_direction = fl_speed * transform.TransformDirection(V3_move_direction);

            // if the jump key is pressed add jump force to the direction vector
            if (Input.GetButton("Jump")) V3_move_direction.y = fl_jump_force;
        }
        else
        {   // Add fl_gravity to the direction vector
            V3_move_direction.y -= fl_gravity * Time.deltaTime;
        }

        // Move the character controller with the direction vector
        CC_PC.Move(V3_move_direction * Time.deltaTime);

    }// -----
 
    //-------------------------------------------------------------------------
    // Simple PC Movement with a chracter controller
    // David Dorrington, UEL Game 2017
}//================
