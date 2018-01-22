// ----------------------------------------------------------------------
// --------------------  Rigifbody expolive forst
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_RB_Impulse : MonoBehaviour {

    public Vector3 v3_explosive_force = Vector3.up; 
    public float radius = 5.0F;
    public float power = 1000.0F;

  	
	// Update is called once per frame
	void Update () {		
        
        if ( Input.GetKeyDown("e") )
             {
                Vector3 explosionPos = transform.position;
                Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
                foreach (Collider hit in colliders)
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (rb != null)
                        rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
                }         

            }
	}//-----
}//==========
