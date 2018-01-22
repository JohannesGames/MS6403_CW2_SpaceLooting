// -------------------- NW Bullet ---------------
// -------------------- David Dorrington, UEL Games, 2017
// ----------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class DD_NW_Bullet : MonoBehaviour
{

    //-------------------------------------------------------------------------
    // Variables
    public float fl_range = 20;
    public float fl_speed = 10;
    public float fl_damage = 20;
    private Rigidbody rb_bullet;

    //-------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        // Destroy this object when range is met
        Destroy(gameObject, fl_range / fl_speed);

        rb_bullet = GetComponent<Rigidbody>();
        rb_bullet.velocity = fl_speed * transform.TransformDirection(Vector3.forward);


    }//-----

 
    //-------------------------------------------------------------------------
    private void OnCollisionEnter(Collision _col)
    {

        _col.collider.gameObject.SendMessage("Damage", fl_damage, SendMessageOptions.DontRequireReceiver);

        // Remove the projectile from the scene when it hits something
        Destroy(gameObject);
    }//-----

}//===============

