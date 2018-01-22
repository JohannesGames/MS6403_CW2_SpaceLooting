// ----------------------------------------------------------------------
// -------------------- 3D Game Manager
// -------------------- David Dorrington, UEL Games, 2016
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class DD_GM : MonoBehaviour {

    //-------------------------------------------------------------------------
    public static string st_game_status = "free";
    public static int in_score;
    Text text_status;
    


    //-------------------------------------------------------------------------
    // Use this for initialization
    void Start ()
    {
        text_status = GameObject.Find("Status_Text").GetComponent<Text>();
        
    }//-----


    //-------------------------------------------------------------------------
    // Update is called once per frame
    void Update ()
    {
        GameControl();
        UpdateGUI();
    }//-----


    //-------------------------------------------------------------------------
    void GameControl()
    {
        if (Input.GetKeyDown("p"))
        {
            if (st_game_status == "free") st_game_status = "paused";
           else if (st_game_status == "paused") st_game_status = "free";
        }
    }//-----


    //-------------------------------------------------------------------------
    void UpdateGUI()
    {
        text_status.text = "GameState: " + st_game_status;
        text_status.text += "\nScore: " + in_score.ToString();
    }//-----

}//===========
