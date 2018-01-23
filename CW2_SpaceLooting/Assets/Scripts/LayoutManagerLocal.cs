using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class LayoutManagerLocal : MonoBehaviour
{
    PCControl pc;

    // Room prefabs
    [Header("Room Prefabs")]
    [Tooltip("0: Crossroad | Must be identical to LayoutManager!!")]
    public Room[] allRooms; // must be identical to LayoutManager

    void Start()
    {
        pc = GetComponent<PCControl>();
    }

    public void SpawnRoom(int roomIndex, Vector3 pos)
    {
        Room newRoom = Instantiate(allRooms[roomIndex], pos, Quaternion.identity);
        
    }
}
