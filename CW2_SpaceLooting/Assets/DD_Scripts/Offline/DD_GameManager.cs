// ----------------------------------------------------------------------
// -------------------- GameManager
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_GameManager : MonoBehaviour {
    // ----------------------------------------------------------------------
    public static int in_shots_fired;

    private GameObject go_message_panel;

    // ----------------------------------------------------------------------
    // Use this for initialization
    void Start () {

        in_shots_fired = PlayerPrefs.GetInt("in_shots");

        go_message_panel = GameObject.Find("GameManager/GUI/SmallMessage");
        go_message_panel.SetActive(false);

    }//-----


    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update ()
    {       
    }//-----
     // ----------------------------------------------------------------------
    void OnDestroy()
    {
        // Player Prefs Test
        PlayerPrefs.SetInt("in_shots", in_shots_fired);
        PlayerPrefs.Save();
    }//------

}//==========
