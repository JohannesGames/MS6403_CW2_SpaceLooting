using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Room : MonoBehaviour
{
    [SerializeField]
    public struct RoomDoor
    {
        bool door;
        bool roomBeyond;
    }

    public string roomName = "crossroad";

    [Tooltip("In order: top, left, right, bottom")]
    public RoomDoor[] doors;

    [Tooltip("0 means no explicit limit and percentLimit is checked")]
    public int limit = 0;

    [Tooltip("0 means no limit on number of this type of room as percentage of the total number of rooms")]
    public int percentLimit = 0;

    public bool mustHaveOne;

    [Header("Loot")]
    public Transform[] lootDrops;

    public Transform[] containersInRoom;

    public NavMeshSurface surface;
}
