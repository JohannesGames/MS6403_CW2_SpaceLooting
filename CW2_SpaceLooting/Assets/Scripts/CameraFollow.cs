using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FL_CameraSpeed = 3;
    public GameObject GO_PC;
    public Camera mainCamera;
    public Camera pcCamera;
    
    void Update()
    {
        if (GO_PC)
        {
            transform.position = Vector3.Lerp(transform.position, GO_PC.transform.position, Time.deltaTime * FL_CameraSpeed);
        }
    }
}
