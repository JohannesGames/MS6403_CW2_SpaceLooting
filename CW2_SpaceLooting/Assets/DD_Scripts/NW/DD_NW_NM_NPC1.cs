using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Networking;

public class DD_NW_NM_NPC1 : NetworkBehaviour
{

    // ----------------------------------------------------------------------
    public Transform tx_target;
    private Vector3 v3_destination;
    private NavMeshAgent nm_agent;
    public float fl_range = 8;


    //-------------------------------------
    [SyncVar]
    private Vector3 v3_syncPos;
    [SyncVar]
    private float fl_syncYRot;
    [SerializeField]
    private float fl_lerpRate = 10;


    // --------------------------------------
    public const int in_max_HP = 100;
    [SyncVar]
    public int in_HP = in_max_HP;


    public GameObject go_projectile;
    public float fl_cool_down = 1;
    private float fl_next_shot_time;


    // ----------------------------------------------------------------------
    void Start()
    {
        if (!isServer)
        {
            return;
        }

        nm_agent = GetComponent<NavMeshAgent>();
    }//-----




    // ----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            LocalPosUpdate();
            return;            
        }
        
        // Stop when close to Target and attack
        if (tx_target)
        {
            if (Vector3.Distance(tx_target.position, transform.position) < fl_range)
            {
                nm_agent.isStopped = true;
                Attack();
            }
            else
            {
                nm_agent.isStopped = false;
            }
        }

        FindTarget();
        // Synch Position on sever
        Cmd_ProvidePositionToServer(transform.position, transform.localEulerAngles.y);

        // Display HP in the attached text mesh component
        GetComponentInChildren<TextMesh>().text = Mathf.Round(in_HP).ToString();
    }//-----

    // ----------------------------------------------------------------------
    [Command]
    void Cmd_ProvidePositionToServer(Vector3 pos, float rot)
    {
        v3_syncPos = pos;
        fl_syncYRot = rot;
    }//------
    // ----------------------------------------------------------------------
    void LocalPosUpdate()
    {
        // Update Local Position
        transform.position = Vector3.Lerp(transform.position, v3_syncPos, Time.deltaTime * fl_lerpRate);
        Vector3 _v3_newrot = new Vector3(0, fl_syncYRot, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_v3_newrot), Time.deltaTime * fl_lerpRate);

        // Display HP in the attached text mesh component
        GetComponentInChildren<TextMesh>().text = Mathf.Round(in_HP).ToString();

    }//-----


    // ----------------------------------------------------------------------
    void Attack()
    {
        transform.LookAt(tx_target.position);

        if (fl_next_shot_time < Time.time)
        {
            CmdFireBullet();
            fl_next_shot_time = Time.time + fl_cool_down;
        }
    }//-----

    // ----------------------------------------------------------------------
    [Command]
    void CmdFireBullet()
    {
        // Create a bullet and reset the shot timer

        var _bullet = (GameObject)Instantiate(go_projectile, transform.position + transform.TransformDirection(new Vector3(0, 1, 1.5F)), transform.rotation);
        fl_next_shot_time = Time.time + fl_cool_down;

        NetworkServer.Spawn(_bullet);

    }//-----




    // ----------------------------------------------------------------------
    void FindTarget()
    {
        if (!tx_target || Vector3.Distance(tx_target.position, v3_destination) > 1)
        {
            // temp variables
            float _dist = Mathf.Infinity;
            GameObject _GO_nearest = null;
            GameObject[] _go_Players = GameObject.FindGameObjectsWithTag("Player");

            // Are there any tagged targets in the scene?
            if (_go_Players.Length > 0)
            {
                // Loop through the list of targets
                foreach (GameObject _GO in _go_Players)
                {
                    float _cur_dist = Vector3.Distance(_GO.transform.position, transform.position);
                    if (_cur_dist < _dist)
                    {
                        _GO_nearest = _GO;
                        _dist = _cur_dist;
                    }
                }

                // Set the Target
                tx_target = _GO_nearest.transform;
                v3_destination = tx_target.position;
                nm_agent.destination = v3_destination;
            }
        }
    }//-----



    // ----------------------------------------------------------------------
    public void Damage(int _damage_amount)
    {
        if (!isServer) return;

        in_HP -= _damage_amount;

        if (in_HP <= 0)
        {
            in_HP = in_max_HP;
            
            transform.position = Vector3.zero;
        }
    }//-----

   
  

}//========