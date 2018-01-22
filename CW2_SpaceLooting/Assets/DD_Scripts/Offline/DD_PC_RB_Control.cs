// ----------------------------------------------------------------------
// -------------------- PC Rigibody Control
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_PC_RB_Control : MonoBehaviour
{
    public float fl_speed = 6;
    private float fl_initial_speed;
    public float fl_jump_force = 500;
    public float fl_mouse_rotation_rate = 180;
    private Rigidbody rb_PC;
    public bool bl_grounded;
    public Vector3 v3_temp;

    // ----------------------------------------------------------------------
    private void Start()
    {
        rb_PC = GetComponent<Rigidbody>();
        fl_initial_speed = fl_speed;
    }//-----

    // ----------------------------------------------------------------------
    void FixedUpdate()
    {
        MovePC();
        CheckGrounded();
    }//-----

    // ----------------------------------------------------------------------
    private void MovePC()
    {
        // Mouse Rotate
        transform.Rotate(0, fl_mouse_rotation_rate * Time.deltaTime * Input.GetAxis("Mouse X"), 0);

        //calculate the velocity vector
        Vector3 _v3_move_direction = transform.position + (fl_speed * Time.deltaTime * transform.TransformDirection( new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical") )));
        
        // Apply movement vector to the rigid body
        rb_PC.MovePosition(_v3_move_direction);

        // Jump if Grounded
        if (bl_grounded)
        { 
            if (Input.GetKeyDown(KeyCode.Space))
                rb_PC.AddForce(Vector3.up * fl_jump_force);
        }

                // If the run key pressed double the speed
        if (Input.GetKey(KeyCode.LeftShift))
            fl_speed = fl_initial_speed * 2;
        else if (Input.GetKey(KeyCode.LeftControl))
            fl_speed = fl_initial_speed / 2;
        else
            fl_speed = fl_initial_speed;


    }//-----


    // ----------------------------------------------------------------------
    private void CheckGrounded()
    {
        RaycastHit hitInfo;
      
        if (Physics.SphereCast(transform.position, 0.2F, Vector3.down , out hitInfo, 0.85F))       
            bl_grounded = true;               
        else      
            bl_grounded = false;     
    }//------ 

}//==========