using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PCControl : MonoBehaviour
{
    public GameObject GO_CameraContainer;
    public float FL_Gravity;
    CameraFollow CF_Camera;
    Vector2 V2_FingerPosition;
    NavMeshAgent NMA_PC;
    CharacterController CC;
    

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
                NMA_PC.SetDestination(hit.point);
            }
        }
    }
}
