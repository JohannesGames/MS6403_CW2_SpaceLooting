﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PCControl : MonoBehaviour
{
    public GameObject GO_CameraContainer;
    public Light LI_Point;
    public float FL_Gravity;
    public LayerMask LM_Ray;    //the layermask for direction and item selection
    CameraFollow CF_Camera;
    Vector2 V2_FingerPosition;
    NavMeshAgent NMA_PC;
    CharacterController CC;
    public GameObject GO_PickupNext = null;   //the object the PC is moving towards
    public List<Collider> CO_InRadius = new List<Collider>();
    public float FL_Reach = 1.5f;   //reach of PC



    void Start()
    {
        CF_Camera = Instantiate(GO_CameraContainer, transform.position, transform.rotation).GetComponent<CameraFollow>();
        CF_Camera.GO_PC = gameObject;
        NMA_PC = GetComponent<NavMeshAgent>();
        CC = GetComponent<CharacterController>();
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

        if (Input.GetButtonDown("Fire1"))   //once finger hits screen take position
        {
            V2_FingerPosition = Input.mousePosition;
        }

        if (Input.GetButtonUp("Fire1"))     //once finger leaves screen move character to specified point
        {
            CheckInput();
        }
    }

    void LateUpdate()
    {
        CO_InRadius.Clear();
    }

    void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(V2_FingerPosition);
        int pickupIndex = LayerMask.NameToLayer("Pickup");
        int floorIndex = LayerMask.NameToLayer("Floor");    //only check "Pickup" and "Floor" layers

        if (pickupIndex == -1 || floorIndex == -1)
            Debug.LogError("Layers incorrectly set up");
        else
        {
            RaycastHit hit;
            int layermask = (1 << pickupIndex | 1 <<floorIndex);    //raycast to "Pickup" only
            if (Physics.Raycast(ray, out hit, layermask))
            {
                if (hit.transform.gameObject.layer == 10)   //if its a pickup, make it the next item to pick up
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

    void CheckForPickups()  //check if PC is within reach of any pickups
    {
        int layerIndex = LayerMask.NameToLayer("Pickup");   //check only for pickups
        if (layerIndex == -1)
            Debug.LogError("No layer called \"Pickup\"");
        int layermask = 1 << layerIndex;
        Collider[] allInRadius = Physics.OverlapSphere(transform.position, FL_Reach, layermask, QueryTriggerInteraction.Ignore);

        if (allInRadius.Length > 0)
        {
            for (int i = 0; i < allInRadius.Length; i++)
            {
                CO_InRadius.Add(allInRadius[i]);
            }
        }
    }
}
