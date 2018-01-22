// ----------------------------------------------------------------------
// -------------------- 3D Projectile
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_Projectile : MonoBehaviour {

    // ----------------------------------------------------------------------
    // Variables

    public float fl_range = 30;
    public float fl_speed = 10;
    public float fl_damage = 10;

    private Rigidbody RB_projectile;

    // ----------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, fl_range / fl_speed);
        RB_projectile = GetComponent<Rigidbody>();
        RB_projectile.velocity = fl_speed * transform.TransformDirection(Vector3.forward);
    } //-----

    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {


    } //-----


    void OnCollisionEnter(Collision _col_arrow_hit)
    {
        _col_arrow_hit.collider.gameObject.SendMessage("Damage", fl_damage, SendMessageOptions.DontRequireReceiver);
       // Destroy(gameObject);
    }

}//==========
