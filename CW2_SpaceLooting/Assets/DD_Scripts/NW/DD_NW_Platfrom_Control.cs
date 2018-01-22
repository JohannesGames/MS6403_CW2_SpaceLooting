// ----------------------------------------------------------------------
// -------------------- 3D Platfrom Anim Control
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DD_NW_Platfrom_Control : NetworkBehaviour
{
    [SyncVar]
    public bool bl_stage1;
    [SyncVar]
    public bool bl_stage2;
    [SyncVar]
    public bool bl_stage3;

    private Animator an_platform;

    // Use this for initialization

    void Start()
    {
        an_platform = GetComponent<Animator>();        
    }//-----

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        if (!an_platform) an_platform = GameObject.Find("anim_platform").GetComponent<Animator>();

        if (bl_stage1) an_platform.SetBool("bl_stage_01", true);
        if (bl_stage2) an_platform.SetBool("bl_stage_02", true);
        if (bl_stage3) an_platform.SetBool("bl_stage_03", true);
        
    }//-----

}//=========

