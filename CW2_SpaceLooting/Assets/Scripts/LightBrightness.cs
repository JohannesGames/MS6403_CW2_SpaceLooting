using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBrightness : MonoBehaviour
{
    public bool BL_Lerp;    //if this is false the light flickers
    bool BL_IsLerpUp = true;
    public float FL_LerpSpeed;
    public float FL_MaxIntensity;
    public float FL_MinIntensity;
    Light LI;
    void Start()
    {
        LI = GetComponent<Light>();
        LI.intensity = FL_MinIntensity;
    }
    
    void Update()
    {
        if (BL_Lerp)
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
    }
}
