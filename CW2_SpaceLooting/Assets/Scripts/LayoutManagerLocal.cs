using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class LayoutManagerLocal : MonoBehaviour
{
    PCControl pc;
    // Navmesh
    [Header("Navigation")]
    public List<NavMeshSurface> allSurfaces = new List<NavMeshSurface>();

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

        if (newRoom.surface)
        {
            allSurfaces.Add(newRoom.surface);
        }
        else
        {
            Debug.LogError("No surface on " + newRoom.roomName);
        }
    }

    public void BuildNavmeshes()
    {
        //StartCoroutine(BuildNevMeshAsync());
    }

    IEnumerator BuildNevMeshAsync()
    {

        print("building navmesh number 1");
        if (allSurfaces[0]) allSurfaces[0].BuildNavMesh();
        yield return new WaitForSeconds(.1f);

        //for (int i = 0; i < allSurfaces.Count; i++)
        //{
        //    print("building navmesh number " + (i + 1));
        //    if (allSurfaces[i]) allSurfaces[i].BuildNavMesh();
        //    yield return new WaitForSeconds(.1f);
        //}
        pc.NMA_PC.enabled = true;
    }
}
