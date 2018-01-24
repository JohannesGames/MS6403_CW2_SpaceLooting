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
    public List<Container> allContainers = new List<Container>();
    public List<Transform> allContainerLocations = new List<Transform>();
    public List<Transform> allLootPoints = new List<Transform>();

    void Start()
    {
        pc = GetComponent<PCControl>();
    }

    public void SpawnRoom(int roomIndex, Vector3 pos)
    {
        Room newRoom = Instantiate(allRooms[roomIndex], pos, Quaternion.identity);

        foreach (Transform item in newRoom.containersInRoom)
        {
            allContainerLocations.Add(item);
        }

        foreach (Transform item in newRoom.lootDrops)
        {
            allLootPoints.Add(item);
        }
    }

    public void DecidePickupLocation(PCControl.ItemPickups ip)
    {
        int chosenIndex = Random.Range(0, (allContainers.Count - 1) + allLootPoints.Count); // from all possible loot drops one index is chosen

        if (chosenIndex > allContainers.Count - 1)  // if it isn't in a container
        {
            chosenIndex -= allContainers.Count - 1;
            pc.CmdSpawnPickupInLootPoint(allLootPoints[chosenIndex].position, ip);
        }
        else
        {
            pc.CmdSpawnPickupInContainer(allContainers[chosenIndex].gameObject, ip);
        }
    }
}
