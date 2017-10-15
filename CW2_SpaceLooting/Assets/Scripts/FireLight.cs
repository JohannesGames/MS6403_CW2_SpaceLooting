using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLight : MonoBehaviour
{
    public Color[] CO_Lights;
    Light li;
    float FL_Intensity;
    float FL_FlickerSpeed;
    
    void Start()
    {
        li = GetComponent<Light>();
        li.color = CO_Lights[Random.Range(0, CO_Lights.Length - 1)];
        FL_Intensity = li.intensity;
        FL_FlickerSpeed = Random.Range(3, 10);
    }

    // Update is called once per frame
    void Update()
    {
        li.intensity = Mathf.PingPong(Time.time * FL_FlickerSpeed, FL_Intensity);
    }
}
