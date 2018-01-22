// -------------------- NW PC Control ---------------
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DD_PC_Network_Control : NetworkBehaviour
{
    // ----------------------------------------------------------------------
    public float fl_speed = 6.0F;
    public float fl_turn_rate = 45;
    public float fl_jump_force = 8.0F;
    public float fl_gravity = 20.0F;

    private Vector3 V3_moveDirection = Vector3.zero;
    private bool bl_climbing;
    private float fl_initial_speed;
    private CharacterController CC_PC;

       
    public GameObject go_projectile;
    public float fl_cool_down = 1;
    private float fl_next_shot_time;

    //-------------------------------------------------------------------------
  

    // ----------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        // Is the spawned PC not the local object the Player Controls
        if (!isLocalPlayer)
        {
            // Remove the PC camera from NW PLayer
            Destroy(transform.Find("PC_Cam").gameObject);

            // Colour other Players
            transform.Find("AlphaHighResMeshes/Alpha_HighTorsoGeo").gameObject.GetComponent<Renderer>().material.color = Color.red;
            transform.Find("AlphaHighResMeshes/Alpha_HighJointsGeo").gameObject.GetComponent<Renderer>().material.color = Color.red;
            transform.Find("AlphaHighResMeshes/Alpha_HighLimbsGeo").gameObject.GetComponent<Renderer>().material.color = Color.red;
        }

        // Local Player Start Conditions
        go_cam = transform.Find("PC_Cam").gameObject;

        // Find the attached Character Controller
        CC_PC = GetComponent<CharacterController>();

        // Set the initial speed
        fl_initial_speed = fl_speed;

    }//-----



    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        // Is the spawned PC not the local object the Player Controls
        if (!isLocalPlayer)
        {
            return;
        }

        // Local Player Functions
        MouseLook();
        MovePC();

        if (Input.GetButtonDown("Fire1") && Time.time > fl_next_shot_time)
        {
            CmdFireBullet();
        }


        if (Input.GetKeyDown("k") )
        {
            CmdSpawnNPC();
        }

    }//-----


    public GameObject go_NPC;

    // ----------------------------------------------------------------------
    [Command]
    void CmdSpawnNPC()
    {
        // Create a bullet and reset the shot timer

        var _npc = (GameObject)Instantiate(go_NPC, Vector3.zero , transform.rotation);
        fl_next_shot_time = Time.time + fl_cool_down;

        NetworkServer.Spawn(_npc);

    }//-----




    // ----------------------------------------------------------------------
    [Command]
    void CmdFireBullet()
    {      
            // Create a bullet and reset the shot timer

        var _bullet = (GameObject)Instantiate(go_projectile, transform.position + transform.TransformDirection(new Vector3(0, 1, 1.5F)), transform.rotation);
            fl_next_shot_time = Time.time + fl_cool_down;

        NetworkServer.Spawn(_bullet);
       
    }//-----


    // ----------------------------------------------------------------------
    void MovePC()
    {

        // If the run key pressed double the speed
        if (Input.GetKey(KeyCode.LeftShift)) fl_speed = fl_initial_speed * 2; else fl_speed = fl_initial_speed;

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
    }//-----


    // ----------------------------------------------------------------------
    // Cam Postion
    public float fl_min_cam_height = -1F;
    public float fl_max_cam_height = 3F;
    public float fl_cam_distance = 2.5F;
    public float fl_mouse_turn_rate = 90;
    public float fl_cam_speed = 2;
    public GameObject go_cam;

    // Mouse Look ==================================================================================
    void MouseLook()
    {
        // Zoom in and out with Mouse Scroll
        if (Input.mouseScrollDelta.y > 0 && fl_cam_distance > 0.5F) fl_cam_distance -= 0.2F;
        if (Input.mouseScrollDelta.y < 0 && fl_cam_distance < 3) fl_cam_distance += 0.2F;


        // Mouse Rotate
        transform.Rotate(0, 3 * fl_mouse_turn_rate * Time.deltaTime * Input.GetAxis("Mouse X"), 0);

        // look at PC Object
        go_cam.transform.LookAt(transform.position + new Vector3(0, 1, 0));

        // Move the Camera
        go_cam.transform.localPosition = new Vector3(0, go_cam.transform.localPosition.y, -fl_cam_distance);

        if (Input.GetAxis("Mouse Y") > 0 && go_cam.transform.localPosition.y > fl_min_cam_height) go_cam.transform.Translate(0, -fl_cam_speed * Time.deltaTime, 0);
        if (Input.GetAxis("Mouse Y") < 0 && go_cam.transform.localPosition.y < fl_max_cam_height) go_cam.transform.Translate(0, fl_cam_speed * Time.deltaTime, 0);

    }//-----


}//==========
