using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotSpeed = 90;
    private float appliedRotSpeed;
    public bool fluctuate = true;

    public enum DirectionRot
    {
        up,
        forward,
        left
    }
    public DirectionRot rotationAxis = DirectionRot.up;

    private void Start()
    {
        if (!fluctuate)
        {
            appliedRotSpeed = rotSpeed;
        }
    }


    void Update()
    {
        if (fluctuate)
        {
            appliedRotSpeed = Random.Range(rotSpeed * .01f, rotSpeed * 1.5f);
        }

        switch (rotationAxis)
        {
            case DirectionRot.up:
                transform.Rotate(Vector3.up, Time.deltaTime * appliedRotSpeed);
                break;
            case DirectionRot.forward:
                transform.Rotate(Vector3.forward, Time.deltaTime * appliedRotSpeed);
                break;
            case DirectionRot.left:
                transform.Rotate(Vector3.left, Time.deltaTime * appliedRotSpeed);
                break;
        }
    }
}
