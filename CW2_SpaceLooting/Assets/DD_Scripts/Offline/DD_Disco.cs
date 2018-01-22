// ----------------------------------------------------------------------
// -------------------- 3D Disco Floor  - Dynamics
// -------------------- David Dorrington, UEL Games, 2016
// ----------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class DD_Disco : MonoBehaviour {

    // ----------------------------------------------------------------------
    
    public Texture[] tex_emit;
    public float fl_delay = 1;
    private Material mat_attached;
    
 

    // ----------------------------------------------------------------------
	void Start () {
        // Get attached material renderer
        mat_attached = GetComponent<Renderer>().material;
         

        if (tex_emit.Length > 0)
            InvokeRepeating("TextureCyle", fl_delay, fl_delay);        
	}//---
    
      
    
    // ----------------------------------------------------------------------
    void TextureCyle()
    {
        int _in_index = Random.Range(0, tex_emit.Length);
                    
        mat_attached.SetTexture("_EmissionMap", tex_emit[_in_index]);            

    }//-----
    
}//===========
