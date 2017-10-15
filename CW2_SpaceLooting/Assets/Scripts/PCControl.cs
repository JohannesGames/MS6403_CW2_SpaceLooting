using System.Collections;
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
    GameObject GO_PickupNext;   //the object the PC is moving towards
    

    void Start()
    {
        CF_Camera = Instantiate(GO_CameraContainer, transform.position, transform.rotation).GetComponent<CameraFollow>();
        CF_Camera.GO_PC = gameObject;
        NMA_PC = GetComponent<NavMeshAgent>();
        CC = GetComponent<CharacterController>();
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
            Ray ray = Camera.main.ScreenPointToRay(V2_FingerPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == 10)   //is it a pickup?
                    GO_PickupNext = hit.transform.gameObject;
                NMA_PC.SetDestination(hit.point);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 8)  //if PC is in the pod room, turn off light
            LI_Point.enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 10)
        {
            if (GO_PickupNext == col.transform.parent.gameObject)
            {
                Debug.Log("Pickup!");
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        //if (col.gameObject.layer == 8)  //if PC leaves pod room, turn on light
        //    LI_Point.enabled = true;
    }
}
