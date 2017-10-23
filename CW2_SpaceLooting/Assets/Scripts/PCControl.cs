﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PCControl : MonoBehaviour
{
    public bool isInMenu;
    public GameObject GO_CameraContainer;
    public Light LI_Point;
    public float FL_Gravity;
    CameraFollow CF_Camera;
    Vector2 V2_FingerPosition;  //where the player presses the screen
    NavMeshAgent NMA_PC;
    float speedNav; //the max navmeshagent speed of the PC
    CharacterController CC;
    PCInventory pcI;
    HUDManager hM;
    public GameObject GO_PickupNext = null;   //the object the PC is moving towards
    public List<Collider> CO_InRadius = new List<Collider>();
    public float FL_Reach = 1.5f;   //reach of PC



    void Start()
    {
        CF_Camera = Instantiate(GO_CameraContainer, transform.position, transform.rotation).GetComponent<CameraFollow>();
        CF_Camera.GO_PC = gameObject;
        NMA_PC = GetComponent<NavMeshAgent>();
        speedNav = NMA_PC.speed;
        CC = GetComponent<CharacterController>();
        pcI = GetComponent<PCInventory>();
        hM = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();

        SetSilhouette();
    }

    void FixedUpdate()
    {
        CheckForPickups();
    }

    void Update()
    {
        if (!CC.isGrounded)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - FL_Gravity, transform.position.z);
            transform.position = pos;
        }
        if (!isInMenu)  //if player is not using a menu take input
        {
            if (Input.GetButtonDown("Fire1"))   //once finger hits screen take position
            {
                V2_FingerPosition = Input.mousePosition;
            }

            if (Input.GetButtonUp("Fire1"))     //once finger leaves screen move character to specified point
            {
                CheckInput();
            }
        }
    }

    void LateUpdate()
    {
        CO_InRadius.Clear();
    }

    public void SetSilhouette() //TODO game manager sets local PC to green (parameter = 1) and other players to red outline
    {
        GetComponentInChildren<PlayerSilhouette>().SetSilhouetteColour(0);
    }

    void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(V2_FingerPosition);
        int pickupIndex = LayerMask.NameToLayer("Pickup");
        int floorIndex = LayerMask.NameToLayer("Floor");    //only check "Pickup" and "Floor" layers
        int containerIndex = LayerMask.NameToLayer("Container");

        if (pickupIndex == -1 || floorIndex == -1 || containerIndex == -1)
            Debug.LogError("Layers incorrectly set up");
        else
        {
            RaycastHit hit;
            int layermask = (1 << pickupIndex | 1 << floorIndex | 1 << containerIndex);    //raycast to "Pickup", "Floor" and "Container" only
            if (Physics.Raycast(ray, out hit, 100, layermask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Pickup") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Container"))   //if its a pickup or container, make it the next item to interact with
                    GO_PickupNext = hit.transform.gameObject;
                NMA_PC.SetDestination(hit.point);
            }
        }

        if (!LI_Point.enabled)   //turn on light with first input
        {
            LI_Point.enabled = true;
            LI_Point.gameObject.SetActive(true);
            LI_Point.transform.parent.gameObject.SetActive(true);
        }
    }

    void CheckForPickups()  //check if PC is within reach of any pickups or containers
    {
        int layerIndex = LayerMask.NameToLayer("Pickup");   //check only for pickups
        if (layerIndex == -1)
            Debug.LogError("No layer called \"Pickup\"");
        int layermask = LayerMask.GetMask("Pickup", "Container");    //1 << layerIndex;
        if (GO_PickupNext != null)
        {
            Collider[] allInRadius = Physics.OverlapSphere(transform.position, FL_Reach, layermask, QueryTriggerInteraction.Ignore);

            if (allInRadius.Length > 0)
            {
                foreach (Collider item in allInRadius)
                    CO_InRadius.Add(item);  //add all nearby items to list
            }

            for (int i = 0; i < allInRadius.Length; i++)
            {
                if (allInRadius[i].gameObject == GO_PickupNext) //if the desired pickup or container is within reach, stop and show inventory screen
                {
                    NMA_PC.ResetPath();
                    hM.OpenSingleItemPanel(GO_PickupNext.GetComponent<Pickup>());  //send what kind of pickup it is to the HUD manager
                    GO_PickupNext = null;
                    //TODO show inventory with encountered object
                }
            }
        }
    }

    public void SetNavSpeed(float mod)
    {

    }
}
