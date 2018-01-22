// ----------------------------------------------------------------------
// -------------------- 3D PC CC Movement - Dynamics
// -------------------- David Dorrington, UEL Games, 2016
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections;


//-------------------------------------------------------------------------
// Components Required 
[RequireComponent(typeof(CharacterController))]


public class DD_PC_Movement : MonoBehaviour
{

	//-------------------------------------------------------------------------
    // ----- Declare Variables 

    public float fl_speed = 6.0F;
    public float fl_turn_rate = 45;
    public float fl_jump_force = 8.0F;
    public float fl_gravity = 20.0F;

    private Vector3 V3_moveDirection = Vector3.zero;
    private bool bl_climbing;
   
    private float fl_initial_speed;
 
    private CharacterController CC_PC;
    public GameObject GO_PC_Gun;
    public static bool bl_PC_crouching;
    public static bool bl_PC_armed;
    public static bool bl_PC_run;



    //-------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
                
        // Find the attached Character Controller
        CC_PC = GetComponent<CharacterController>();


        // Hide the PC Weapon
        GO_PC_Gun.SetActive(false);

        // Set the initial speed
        fl_initial_speed = fl_speed;

    }//-----


    //-------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        // Alive Functions
        if (DD_GM.st_game_status == "free")
        {
            MovePC();
            ChangeState();    
        }

    }//-----



    //-------------------------------------------------------------------------
    // Move PC
    void ChangeState()
    {
        // Crouch -------------------------------
        if (Input.GetKeyDown("c") && bl_PC_armed) bl_PC_crouching = true;
        if (Input.GetKeyDown("v") && bl_PC_armed) bl_PC_crouching = false;

        if (bl_PC_crouching)
        {
            CC_PC.height = 1.35F;
            CC_PC.center = new Vector3(0, -0.3F, 0);
        }
        else
        {
            CC_PC.height = 2F;
            CC_PC.center = new Vector3(0, 0, 0);
        }

        // Arm -------------------------------------
        if (Input.GetKeyDown("1")) 
        {
            bl_PC_armed = true;
            GO_PC_Gun.SetActive(true);
        }

        if (Input.GetKeyDown("2"))
        {
            bl_PC_armed = false;

            GO_PC_Gun.SetActive(false);
        }
    }//-----



    //-------------------------------------------------------------------------
    // Move PC
    void MovePC()
    {
        
        // If the run key pressed double the speed
        if ( !bl_PC_crouching && Input.GetKey(KeyCode.LeftShift) ) fl_speed = fl_initial_speed * 2; else fl_speed = fl_initial_speed;

        //  PC Ground Movement
        if (CC_PC.isGrounded)
        {
            // Add X & Z movement to the direction vector based on Vertical input (W,S or Cursor U,D) 
            V3_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Convert world coordinates to local for the PC and multiple by speed
            V3_moveDirection = fl_speed * transform.TransformDirection(V3_moveDirection);

            // if the jump key is pressed add jump force to the direction vector
            if (Input.GetButton("Jump")) V3_moveDirection.y = fl_jump_force;
        }

        // PC Climb Movement
        if (bl_climbing)
        {
            // Add Y movement to the direction vector based on Vertical input (W,S or Cursor U,D) 
            V3_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            V3_moveDirection = fl_speed / 2 * transform.TransformDirection(V3_moveDirection);
            if (Input.GetButton("Jump")) V3_moveDirection.y = fl_jump_force / 2;
        }
        else
        {
            // Add fl_gravity to the direction vector
            V3_moveDirection.y -= fl_gravity * Time.deltaTime;
        }

        // Move the character controller with the direction vector
        CC_PC.Move(V3_moveDirection * Time.deltaTime);

        
    }// -----


    //-------------------------------------------------------------------------
    // When PC enters a trigger
    void OnTriggerStay(Collider cl_trigger_collider)
    {
        if (cl_trigger_collider.gameObject.tag == "Moving") transform.parent = cl_trigger_collider.transform;
        if (cl_trigger_collider.gameObject.tag == "Climbable") bl_climbing = true;
    }//-----


    //-------------------------------------------------------------------------
    // PC Leaving the Trigger
    void OnTriggerExit(Collider cl_trigger_collider)
    {
        if (cl_trigger_collider.gameObject.tag == "Moving") transform.parent = null;
        if (cl_trigger_collider.gameObject.tag == "Climbable") bl_climbing = false;
    }//-----

}//==========

