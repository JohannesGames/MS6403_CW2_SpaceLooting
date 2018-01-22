using UnityEngine;
using System.Collections;

public class DD_Anim_play : MonoBehaviour
{

    private Animator Ramp_Animator;
    public string st_bool;

    //-------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        // Find the attached Character Controller
        Ramp_Animator = GetComponent<Animator>();

    }//------


    //-------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
            Ramp_Animator.SetBool(st_bool, true);

        // set the animation controller state for the transistion to play the animation node

    }//-----

}//==========
