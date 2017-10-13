using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBrightness : MonoBehaviour
{
    //light lerping variables
    public bool BL_Lerp;    //if this is false the light flickers
    bool BL_IsLerpUp = true;
    public float FL_LerpSpeed;
    private float FL_MaxIntensity;
    public float FL_MinIntensity = 0;
    //light flickering variables
    private bool FL_IsFlickerOn;
    public float FL_FlickerMaxTimeOn;   //maximum time light is on
    public float FL_FlickerMinTimeOn;   //minimum time light is on
    public float FL_FlickerMaxTimeOff;  //maximum time light is off
    public float FL_FlickerMinTimeOff;  //minimum time light is off
    private float FL_FlickerTime;   //how long the light has been on or off
    Light LI;

    void Start()
    {
        LI = GetComponent<Light>();
        FL_MaxIntensity = LI.intensity;
        LI.intensity = FL_MinIntensity;
    }
    
    void Update()
    {
        if (BL_Lerp)    //if light is set to lerp
        {
            if (BL_IsLerpUp && LI.intensity < FL_MaxIntensity)
            {
                LI.intensity += Time.deltaTime * FL_LerpSpeed;
                if (LI.intensity >= FL_MaxIntensity)
                    BL_IsLerpUp = false;
            }
            else if (LI.intensity > FL_MinIntensity)
            {
                LI.intensity -= Time.deltaTime * FL_LerpSpeed;
                if (LI.intensity <= FL_MinIntensity)
                    BL_IsLerpUp = true;
            }
        }
        else    //otherwise turn light on and off
        {
            if (Time.time >= FL_FlickerTime)
            {
                if (FL_IsFlickerOn)
                {
                    LI.intensity = FL_MinIntensity;
                    FL_FlickerTime = Time.time + Random.Range(FL_FlickerMinTimeOn, FL_FlickerMaxTimeOn);
                }
                else
                {
                    LI.intensity = FL_MaxIntensity;
                    FL_FlickerTime = Time.time + Random.Range(FL_FlickerMinTimeOff, FL_FlickerMaxTimeOff);
                }
                FL_IsFlickerOn = !FL_IsFlickerOn;
            }
        }
    }
}
