using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_Simple_Move : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        transform.Rotate(0, 45* Time.deltaTime , 0);
        transform.Translate(0,0,5 * Time.deltaTime);

	}
}
