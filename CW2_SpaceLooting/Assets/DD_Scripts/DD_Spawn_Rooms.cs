using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class DD_Spawn_Rooms : MonoBehaviour
{
    public GameObject[] gos_rooms;
    public int in_size = 5;
    private GameObject[,] gos_spawned;
    public Transform tx_start_position;
    private Vector3 v3_spawn_pos;

    // Use this for initialization
    void Start()
    {
        v3_spawn_pos = tx_start_position.position;
        gos_spawned = new GameObject[in_size, in_size];
        SpawnRooms();
        // BuildNavmesh();
    }//------


    //-----------------------------------------------------------------
    void SpawnRooms()
    {   
        /// Rows
        for (int _rows = 0; _rows < in_size; _rows++)
        {   // Cols
            for (int _cols = 0; _cols < in_size; _cols++)
            {
                

                    int _index = Random.Range(0, gos_rooms.Length);
                    Instantiate(gos_rooms[_index], new Vector3(v3_spawn_pos.x + _cols * 10, v3_spawn_pos.y, v3_spawn_pos.z + _rows * -10), Quaternion.Euler(270, 0, 0));
                    gos_spawned[_rows, _cols] = gos_rooms[_index];

               
            }
        }
    }//----------

}//==========
