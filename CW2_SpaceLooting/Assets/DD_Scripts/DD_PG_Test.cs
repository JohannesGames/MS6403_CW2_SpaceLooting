using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI ;

public class DD_PG_Test : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        //  print(gos_spawned[0,0].GetComponent<DD_Room>().Doors);   
    }




    //-----------------------------------------------------------------
    void SpawnRooms()
    {
        // List of potential rooms
        List<int> _l_in_potential_rooms = new List<int>();

        /// Rows
        for (int _rows = 0; _rows < in_size; _rows++)
        {   // Cols
            for (int _cols = 0; _cols < in_size; _cols++)
            {
                if (_cols == 0) // First Column ---------------------------------------------
                {
                    if (_rows == 0) // First slot - top left 0, 0
                    {
                        int _index = Random.Range(0, gos_rooms.Length);
                        Instantiate(gos_rooms[_index], new Vector3(v3_spawn_pos.x + _cols * 10, v3_spawn_pos.y, v3_spawn_pos.z + _rows * -10), Quaternion.Euler(270, 0, 0));
                        gos_spawned[_rows, _cols] = gos_rooms[_index];

                    }
                    else if (_rows > 0)  // second Row onwards slot 0,1
                    {
                        // Clear List
                        _l_in_potential_rooms.Clear();

                        for (int _index = 0; _index <= gos_rooms.Length - 1; _index++)
                        {
                            // Check potenitial room matches room above bottom door
                            if (gos_rooms[_index].GetComponent<DD_Room>().Doors.x == gos_spawned[_rows - 1, _cols].GetComponent<DD_Room>().Doors.z)
                            {
                                _l_in_potential_rooms.Add(_index);
                            }
                        }                    

                        // Spawn a room from the list of possibles
                        if (_l_in_potential_rooms.Count > 0)
                        {
                            int _index = Random.Range(0, _l_in_potential_rooms.Count);

                            Instantiate(gos_rooms[_l_in_potential_rooms[_index]], new Vector3(v3_spawn_pos.x + _cols * 10, v3_spawn_pos.y, v3_spawn_pos.z + _rows * -10), Quaternion.Euler(270, 0, 0));
                            gos_spawned[_rows, _cols] = gos_rooms[_l_in_potential_rooms[_index]];
                        }
                    }
                }

                //------------------------------------------------------------
                if (_cols > 0) // Second Column onwwards 1
                {

                    if (_rows == 0) // First Row
                    {
                        // Clear List
                        _l_in_potential_rooms.Clear();

                        // Find all possible rooms
                        for (int _index = 0; _index < gos_rooms.Length; _index++)
                        {
                            // Check potenitial room matches room left right door
                            if (gos_rooms[_index].GetComponent<DD_Room>().Doors.w == gos_spawned[_rows, _cols - 1].GetComponent<DD_Room>().Doors.y)
                            {
                                _l_in_potential_rooms.Add(_index);
                            }
                        }

                        // Spawn a room from the list of possibles
                        if (_l_in_potential_rooms.Count > 0)
                        {
                            int _index = Random.Range(0, _l_in_potential_rooms.Count);

                            Instantiate(gos_rooms[_l_in_potential_rooms[_index]], new Vector3(v3_spawn_pos.x + _cols * 10, v3_spawn_pos.y, v3_spawn_pos.z + _rows * -10), Quaternion.Euler(270, 0, 0));
                            gos_spawned[_rows, _cols] = gos_rooms[_l_in_potential_rooms[_index]];
                        }
                    }
                    else if (_rows > 0)
                    {// second Row onwards slot 0,1

                        // Clear List
                        _l_in_potential_rooms.Clear();

                        for (int _index = 0; _index < gos_rooms.Length ; _index++)
                        {
                            // Check potenitial room matches room left right door
                            if (gos_rooms[_index].GetComponent<DD_Room>().Doors.w == gos_spawned[_rows, _cols - 1].GetComponent<DD_Room>().Doors.y)
                            {
                                // Check potenitial room matches room above bottom door
                                if (gos_rooms[_index].GetComponent<DD_Room>().Doors.x == gos_spawned[_rows - 1, _cols].GetComponent<DD_Room>().Doors.z)
                                {
                                    _l_in_potential_rooms.Add(_index);
                                }
                            }
                        }     

                       // print(_l_in_potential_rooms.Count + " Possible rooms");

                        if (_l_in_potential_rooms.Count > 0)
                        {
                            int _index = Random.Range(0, _l_in_potential_rooms.Count);

                            Instantiate(gos_rooms[_l_in_potential_rooms[_index]], new Vector3(v3_spawn_pos.x + _cols * 10, v3_spawn_pos.y, v3_spawn_pos.z + _rows * -10), Quaternion.Euler(270, 0, 0));
                            gos_spawned[_rows, _cols] = gos_rooms[_l_in_potential_rooms[_index]];
                        }                                               

                    }
                }
            }
        }
    }//----------


}//=========

