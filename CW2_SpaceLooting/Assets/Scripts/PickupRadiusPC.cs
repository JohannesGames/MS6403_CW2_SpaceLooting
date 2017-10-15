using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRadiusPC : MonoBehaviour
{
    PCControl pc;

    void Start()
    {
        pc = GetComponentInParent<PCControl>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (pc.GO_PickupNext == col.transform.gameObject)
        {
            Debug.Log("Pickup!");
        }
    }
}
