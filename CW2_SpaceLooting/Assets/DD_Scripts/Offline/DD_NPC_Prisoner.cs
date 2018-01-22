// ----------------------------------------------------------------------
// -------------------- 3D NPC CC Movement - Dynamics
// -------------------- David Dorrington, UEL Games, 2016
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------
// Components Required 
[RequireComponent(typeof(CharacterController))]

public class DD_NPC_Prisoner : MonoBehaviour {

    //-------------------------------------------------------------------------
    // Variables
    public bool bl_chase = true;
    public float fl_dist_max = 10;
    public float fl_dist_min = 3;
    public float fl_speed = 3;
    
    public GameObject GO_target;
    private CharacterController CC_NPC;
    public GameObject GO_NPC_mesh;

    private Animator NPC_Animator;

    // Use this for initialization
    void Start ()
    {
        CC_NPC = GetComponent<CharacterController>();
        NPC_Animator = GO_NPC_mesh.GetComponent<Animator>();
    }//-----

    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        NPC_Move();
        NPC_Animate();
    }//-----

    // ----------------------------------------------------------------------
    void NPC_Move()
    {
        // Is the target in Range
        if (Vector3.Distance(transform.position, GO_target.transform.position) < fl_dist_max && Vector3.Distance(transform.position, GO_target.transform.position) > fl_dist_min)
        {
            // Face the Target
            transform.LookAt(GO_target.transform.position);

            if (Vector3.Distance(transform.position, GO_target.transform.position) > fl_dist_min)
            {
                // Move towards the target
                CC_NPC.SimpleMove(fl_speed * transform.TransformDirection(Vector3.forward));
            }
        }
        else
        {
            CC_NPC.SimpleMove(Vector3.zero);
        }

    }//-----

    void NPC_Animate()
    {


        if (CC_NPC.velocity.x != 0 || CC_NPC.velocity.z != 0)
            NPC_Animator.SetBool("walkF", true);
        else
            NPC_Animator.SetBool("walkF", false);
    }//-----

}//==========
