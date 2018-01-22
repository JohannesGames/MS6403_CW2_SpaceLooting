// ----------------------------------------------------------------------
// -------------------- 3D Particle Control - Dynamics
// -------------------- David Dorrington, UEL Games, 2016
// ----------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DD_Particle_Gun_Control : MonoBehaviour {

    private GameObject GO_PC;
    private ParticleSystem PS_Gun;
    public float fl_activation_distance = 3;
    Text text_message;
    GameObject GO_message_panel;

    // Use this for initialization
    void Start () {

        // Find Game Objects
        GO_PC = GameObject.Find("PC");
        PS_Gun = GetComponent<ParticleSystem>();
        GO_message_panel = GameObject.Find("GameManager/GUI/Small_Message_Panel");
        text_message = GameObject.Find("GameManager/GUI/Small_Message_Panel/Small_Message_Text").GetComponent<Text>();

    }//----
	
	// Update is called once per frame
	void Update () {

        // Is the PC in range to control this
        if (Vector3.Distance(transform.position, GO_PC.transform.position ) < fl_activation_distance)
        {
            GO_message_panel.SetActive(true);
            text_message.text = " Press Y or H to change speed";

            // Change Emit Speed 
            if (Input.GetKey("y")) PS_Gun.startSpeed += 0.1F;
            if (Input.GetKey("h")) PS_Gun.startSpeed -= 0.1F;
            // Rotate Emitter
            if (Input.GetKey("g")) transform.Rotate( -1, 0, 0);
            if (Input.GetKey("j")) transform.Rotate( 1, 0, 0);

            // transform.Rotate(0, 1, 0);

        }
        else if (Vector3.Distance(transform.position, GO_PC.transform.position) < fl_activation_distance + 1)
            GO_message_panel.SetActive(false);

    }//------

}//=-=========
