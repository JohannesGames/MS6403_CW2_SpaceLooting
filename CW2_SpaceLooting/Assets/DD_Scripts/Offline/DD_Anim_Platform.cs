using UnityEngine;
using System.Collections;

public class DD_Anim_Platform : MonoBehaviour {

    private Animator platform_Animator;
    public int in_anim_state;

    //-------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        // Find the attached Character Controller

        platform_Animator = GetComponent<Animator>();


    }//------


    //-------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("p"))
        {
            in_anim_state++;

            platform_Animator.SetInteger("in_state", in_anim_state);
        }
        }//-----
}
