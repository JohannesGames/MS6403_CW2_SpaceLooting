// ----------------------------------------------------------------------
// -------------------- 3D PC Mouse Look - Dynamics
// -------------------- David Dorrington, UEL Games, 2016
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class DD_PC_MouseLook : MonoBehaviour {

    // Cam Postion
    public float fl_min_cam_height = -1F;
    public float fl_max_cam_height = 3F;
    public float fl_cam_distance = 2.5F;
    public float fl_mouse_turn_rate = 90;
    public float fl_cam_speed = 2;
    public bool bl_invert;
    public GameObject go_cam;

   // private DD_3D_Level_Manager LevelManager;

    // ----------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        // Find the Game Objects we need to interact with
        //LevelManager = GameObject.Find("GameManager").GetComponent<DD_3D_Level_Manager>();

    }


    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

        // Alive Functions - of the game in not paused
       // if (LevelManager.st_game_status == "free")
       // {
            MouseLook();

        //}

    }//-----



    // Mouse Look ==================================================================================
    void MouseLook()
    {
        // Zoom in and out with Mouse Scroll
        if (Input.mouseScrollDelta.y > 0 && fl_cam_distance > 0.5F) fl_cam_distance -= 0.2F;
        if (Input.mouseScrollDelta.y < 0 && fl_cam_distance < 3) fl_cam_distance += 0.2F;


        // Mouse Rotate
        transform.Rotate(0, 3 * fl_mouse_turn_rate * Time.deltaTime * Input.GetAxis("Mouse X"), 0);

        // Move Cam up and Down
        if (bl_invert)
        {
            if (Input.GetAxis("Mouse Y") > 0 && go_cam.transform.localPosition.y < fl_max_cam_height) go_cam.transform.Translate(0, fl_cam_speed * Time.deltaTime, 0);
            if (Input.GetAxis("Mouse Y") < 0 && go_cam.transform.localPosition.y > fl_min_cam_height) go_cam.transform.Translate(0, -fl_cam_speed * Time.deltaTime, 0);
        }
        else
        {
            if (Input.GetAxis("Mouse Y") > 0 && go_cam.transform.localPosition.y > fl_min_cam_height) go_cam.transform.Translate(0, -fl_cam_speed * Time.deltaTime, 0);
            if (Input.GetAxis("Mouse Y") < 0 && go_cam.transform.localPosition.y < fl_max_cam_height) go_cam.transform.Translate(0, fl_cam_speed * Time.deltaTime, 0);
        }


        // look at PC Object
        go_cam.transform.LookAt(transform.position + new Vector3(0, 1, 0));

        // Move the Camera
        go_cam.transform.localPosition = new Vector3(0, go_cam.transform.localPosition.y, -fl_cam_distance);

    }//-----

}//==========

