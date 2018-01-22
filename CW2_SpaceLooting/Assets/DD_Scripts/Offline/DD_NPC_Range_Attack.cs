// ----------------------------------------------------------------------
// -------------------- 3D NPC Chase 
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_NPC_Range_Attack : MonoBehaviour {

    // ----------------------------------------------------------------------
    // Combat Variables
    public GameObject GO_projectile;
    public float fl_attack_range = 5;
    public float fl_cool_down = 1;
    public int in_ammo = 100;
    private float fl_delay;
    public bool bl_line_of_sight;

    // Movement
    public string st_target_class = "NPC_02";
    public bool bl_chase = true;
    public float fl_chase_dist_max = 10;
    public float fl_chase_dist_min = 3;
    public float fl_chase_speed = 3;

    public GameObject GO_home;
    public GameObject GO_target;
    private CharacterController CC_NPC;
    private Animator NPC_Animator;


    // ----------------------------------------------------------------------

    // Use this for initialization
    void Start()
    {
        // Find the Game Objects we need to interact with
        CC_NPC = GetComponent<CharacterController>();
        NPC_Animator = GetComponentInChildren<Animator>();


    }//-----

    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

       
            if (!GO_target)
            {
                FindTarget();
            }
            else
            {
                AttackTarget();
                NPC_Move();
            }

        //    NPC_Animate();
      

    }//-----

    // ----------------------------------------------------------------------
    void NPC_Animate()
    {
        // Set walking state for Animator
        if (CC_NPC.velocity.x != 0 || CC_NPC.velocity.z != 0)
            NPC_Animator.SetBool("bl_walk", true);
        else
            NPC_Animator.SetBool("bl_walk", false);
    }//-----




    // ----------------------------------------------------------------------
    void AttackTarget()
    {
        if (in_ammo > 0 && Time.time > fl_delay && Vector3.Distance(transform.position, GO_target.transform.position) < fl_attack_range)
        {
            // Face the Target
            transform.LookAt(GO_target.transform.position);

            // Is the line of sight flag checked? 
            if (bl_line_of_sight)
            {
                // Cast a Ray to check if Target in is view of NPC
                RaycastHit _RC_hit;
                Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _RC_hit, fl_attack_range);

                // if the Target is in sight create an arrow
                if (_RC_hit.collider != null && _RC_hit.collider.gameObject == GO_target)
                {
                    Instantiate(GO_projectile, transform.position + transform.TransformDirection(new Vector3(0, 0.5F, 1.5F)), transform.rotation);
                    fl_delay = Time.time + fl_cool_down;
                    in_ammo--;
                  
                }
            }
            else
            {
                // Shoot at Target even if there is something in the way            
                Instantiate(GO_projectile, transform.position + transform.TransformDirection(new Vector3(0, 0.5F, 1.5F)), transform.rotation);
                fl_delay = Time.time + fl_cool_down;
                in_ammo--;
            }

            DD_GameManager.in_shots_fired++;
        }
    }//------


    // ----------------------------------------------------------------------
    void NPC_Move()
    {
        // Is the target in Range
        if (Vector3.Distance(transform.position, GO_target.transform.position) < fl_chase_dist_max)
        {
            // Face the Target
            transform.LookAt(GO_target.transform.position);
            // is the target not too close
            if (Vector3.Distance(transform.position, GO_target.transform.position) > fl_chase_dist_min)
            {
                // Move towards the target
                CC_NPC.SimpleMove(fl_chase_speed * transform.TransformDirection(Vector3.forward));
            }
            else
            {
                CC_NPC.SimpleMove(Vector3.zero);
            }
        }
        else if (GO_home && Vector3.Distance(transform.position, GO_home.transform.position) > fl_chase_dist_min)
        {
            // Head Home
            transform.LookAt(GO_home.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            CC_NPC.SimpleMove(fl_chase_speed * transform.TransformDirection(Vector3.forward));
        }
        else
        {
            CC_NPC.SimpleMove(Vector3.zero);
        }

    }//-----


    // ----------------------------------------------------------------------
    void FindTarget()
    {
        // temp variables
        float _dist = Mathf.Infinity;
        GameObject _GO_nearest = null;

        // Create a List of potential targets
        GameObject[] _GO_Enemies = GameObject.FindGameObjectsWithTag(st_target_class);

        // Are there any tagged targets in the scene?
        if (_GO_Enemies.Length > 0)
        {
            // Loop through the list of targets
            foreach (GameObject _GO in _GO_Enemies)
            {
                float _cur_dist = Vector2.Distance(_GO.transform.position, transform.position);
                if (_cur_dist < _dist)
                {
                    _GO_nearest = _GO;
                    _dist = _cur_dist;
                }
            }
        }

        // Set the Target
        GO_target = _GO_nearest;
    }//-----

}//==========
