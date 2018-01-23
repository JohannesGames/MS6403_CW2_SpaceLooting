using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class RegisterStartPoint : MonoBehaviour
{
    
    void Awake()
    {
        JB_NetworkManager.RegisterStartPosition(transform);
    }
}
