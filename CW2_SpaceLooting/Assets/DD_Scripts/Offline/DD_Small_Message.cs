// ----------------------------------------------------------------------
// -------------------- 3D Message System
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DD_Small_Message : MonoBehaviour
{     
    // ----------------------------------------------------------------------
    public string st_message;
    public float fl_distance = 1;
    private GameObject go_trigger_object;
    public bool bl_message_on;
    public bool bl_next_button;
    private GameObject go_message_panel;
    private Text text_small;
    public bool bl_destroy_when_done;

    // ----------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        go_trigger_object = GameObject.Find("PC");
        go_message_panel = GameObject.Find("GameManager/GUI/SmallMessage");
        text_small = GameObject.Find("GameManager/GUI/SmallMessage/SmallMessageText").GetComponent<Text>();
    }//-----
     
    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update ()
    {

        // In trigger distance
        if (Vector3.Distance(go_trigger_object.transform.position, transform.position) < fl_distance)
        {
            // Is the panel active
            if (!go_message_panel.activeInHierarchy)
            {
                // Turn on Message Panel
                go_message_panel.SetActive(true);
                text_small.text = st_message;
            }

            // Destroy Panel
            if (Input.GetKeyDown("e"))
            {              
                    go_message_panel.SetActive(false);

                if (bl_destroy_when_done) Destroy(gameObject);                
            }
        }
        else if (go_trigger_object && Vector3.Distance(go_trigger_object.transform.position, transform.position) < fl_distance + 1)
        {
            go_message_panel.SetActive(false);
        }
    }//-----
}//==========
