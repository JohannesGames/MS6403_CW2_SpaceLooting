using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class PCControl : NetworkBehaviour
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
    //PCInventory pcI;
    HUDManager hM;
    public GameObject GO_PickupNext = null;   //the object the PC is moving towards
    public List<Collider> CO_InRadius = new List<Collider>();
    public float FL_Reach = 1.5f;   //reach of PC
    public Transform pcInvenTrans;  //where the inventory is in the hierarchy



    public override void OnStartLocalPlayer()   //is this "override" necessary?
    {
        CF_Camera = Instantiate(GO_CameraContainer, transform.position, transform.rotation).GetComponent<CameraFollow>();
        CF_Camera.GO_PC = gameObject;
        NMA_PC = GetComponent<NavMeshAgent>();
        speedNav = NMA_PC.speed;
        CC = GetComponent<CharacterController>();
        hM = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();

        SetSilhouette();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CheckForPickups();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!CC.isGrounded) //if PC is not grounded move downwards
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - FL_Gravity, transform.position.z);
            transform.position = pos;
        }
        if (!isInMenu)  //if player is not using a menu take input
        {
            TakeInput();
        }
    }

    void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CO_InRadius.Clear();
    }

    public void SetSilhouette() //TODO game manager sets local PC to green (parameter = 1) and other players to red outline
    {
        GetComponentInChildren<PlayerSilhouette>().SetSilhouetteColour(0);
    }

    #region // Input //

    void TakeInput()
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

    void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(V2_FingerPosition);
        int pickupIndex = LayerMask.NameToLayer("Pickup");
        int floorIndex = LayerMask.NameToLayer("Floor");
        int podIndex = LayerMask.NameToLayer("Pod");
        int containerIndex = LayerMask.NameToLayer("Container");    //only check "Pickup", "Floor", "Pod" and "Container" layers

        if (pickupIndex == -1 || floorIndex == -1 || containerIndex == -1 || podIndex == -1)
            Debug.LogError("Layers incorrectly set up");
        else
        {
            RaycastHit hit;
            int layermask = (1 << pickupIndex | 1 << floorIndex | 1 << containerIndex | 1 << podIndex);    //raycast to "Pickup", "Floor", "Pod" and "Container" only
            if (Physics.Raycast(ray, out hit, 100, layermask))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Pickup")
                    || hit.transform.gameObject.layer == LayerMask.NameToLayer("Container")
                    || hit.transform.gameObject.layer == LayerMask.NameToLayer("Pod"))   //if its a pickup or container or pod, make it the next item to interact with
                    GO_PickupNext = hit.transform.gameObject;
                else
                    GO_PickupNext = null;
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
#endregion

    void CheckForPickups()  //check if PC is within reach of any pickups or containers
    {
        int layerIndex = LayerMask.NameToLayer("Pickup");   //check only for pickups
        if (layerIndex == -1)
            Debug.LogError("No layer called \"Pickup\"");
        int layermask = LayerMask.GetMask("Pickup", "Container", "Pod");    //1 << layerIndex;
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
                    NMA_PC.ResetPath(); //stop the PC on the navmesh

                    if (GO_PickupNext.layer == 10)  //if it's a pickup display it in the inventory
                    {
                        Pickup temp = GO_PickupNext.GetComponent<Pickup>();
                        hM.OpenSingleItemPanel(temp);  //send what kind of pickup it is to the HUD manager
                    }
                    else if (GO_PickupNext.layer == 15)    //if it's a container display it in the inventory
                    {
                        Container temp = GO_PickupNext.GetComponent<Container>();
                        hM.OpenContainerPanel(temp);
                    }
                    else    //if it's the pod, open repair screen
                    {
                        hM.OpenRepairPodPanel();
                    }


                    GO_PickupNext = null;
                    break;
                }
            }
        }
    }

    public void SetNavSpeed(float mod)
    {
        NMA_PC.speed = speedNav * mod;
    }
}
