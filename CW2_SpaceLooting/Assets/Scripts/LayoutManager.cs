using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

// http://www.theappguruz.com/blog/unity-csv-parsing-unity used for reference

public class PickupSpawner
{
    public PickupSpawner(string _name, int _rarity)
    {
        pickupName = _name;
        rarity = _rarity;
    }
    public string pickupName;
    public int rarity;
}

public class PrePickup
{
    public string itemName;

    public enum ItemType
    {
        tool,
        component,
        boost
    }

    public ItemType pickupType;
}

public class LayoutManager : NetworkBehaviour
{

    public Pickup pickupPrefab;
    public int playerNumber = 2;
    public int toolsRequired = 3;
    public int compsRequired = 6;
    public int boostsPerPlayer = 5;
    public int itemMulitplier = 2;  // how many times over should ALL the players be able to leave (how many items should there be?)

    public TextAsset toolCSV;           //names and rarity values of tools
    public TextAsset compCSV;
    public TextAsset boostCSV;
    public List<PickupSpawner> spawnList = new List<PickupSpawner>();
    private List<int> serialsUsed = new List<int>();
    private char lineSeperater = '\n';
    private char fieldSeperator = ',';

    // Possible Layouts
    [Header("Possible Layouts")]
    public TextAsset[] twoPlayerLayouts;
    public TextAsset[] threePlayerLayouts;
    public TextAsset[] fourPlayerLayouts;
    public TextAsset[] fivePlayerLayouts;
    public TextAsset[] sixPlayerLayouts;
    public TextAsset[] sevenPlayerLayouts;
    public TextAsset[] eightPlayerLayouts;
    public TextAsset[] ninePlayerLayouts;

    private TextAsset currentLayout;
    private int roomRows;
    private int roomColumns;
    private List<int> mustHaveRooms = new List<int>();                              // the array indices of the rooms in the allRooms Room[] array that must be included at least once
    private Dictionary<string, int> roomTypeCount = new Dictionary<string, int>();  // counts how many of each room has been spawned to check room number limits
    private List<string> crossroadCoordinates = new List<string>();
    private int podRoomRow;
    private int podRoomColumn;
    private int totalRoomNumber;
    private int totalRoomCount = 0; // TODO delete when unnecessary

    // Navmesh
    public List<GameObject> allPlayers = new List<GameObject>();
    [Header("Navigation")]
    public List<NavMeshSurface> allSurfaces = new List<NavMeshSurface>();

    // Room prefabs
    [Header("Room Prefabs")]
    [Tooltip("0: Crossroad")]
    public Room[] allRooms;

    //public Room crossroad;
    //public Room canteen;
    //public Room barracks;
    //public Room medBay;
    //public Room storage;
    //public Room weaponStation;
    //public Room bridge;
    //public Room comms;
    
    void Start()
    {
        if (!isServer)
        {
            return;
        }
        DontDestroyOnLoad(this);

        //foreach (InventoryPickup.ItemType _type in System.Enum.GetValues(typeof(InventoryPickup.ItemType))) // iterate through all pickup types and spawn all the objects
        //{
        //    SpawnPickups(_type);
        //}
    }

    public void BeginTheSpawning(List<GameObject> _allPlayers)
    {
        if (!isServer)
        {
            return;
        }

        foreach (GameObject item in _allPlayers)
        {
            allPlayers.Add(item);
        }

        SpawnRooms();

        SpawnPickups(InventoryPickup.ItemType.tool);
        SpawnPickups(InventoryPickup.ItemType.component);
    }

    #region Room Spawning
    
    void SpawnRooms()
    {
        print("spawning rooms");
        currentLayout = PickLayout(playerNumber);                                   // pick a CSV layout file depending on the number of players
        string[,] finalRoomLayout = SplitRoomCSV();                                 // get a 2D string array describing the level layout
        List<string> roomCoordinates = new List<string>();                              // where all the rooms are
        int roomCount = 0;
        GetAllRoomIndices(finalRoomLayout, ref roomCoordinates, ref roomCount);         // find where all the rooms are
        SpawnCrossroads();
        SpawnMustHaves(ref roomCoordinates);
        SpawnAllRooms(ref roomCoordinates);
        RpcBuildNavmeshes();
        
    }

    TextAsset PickLayout(int playerNum)
    {
        switch (playerNum)
        {
            case 2:
                return twoPlayerLayouts[Random.Range(0,twoPlayerLayouts.Length)];
            case 3:
                return threePlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
            case 4:
                return fourPlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
            case 5:
                return fivePlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
            case 6:
                return sixPlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
            case 7:
                return sevenPlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
            case 8:
                return eightPlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
            case 9:
                return ninePlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
            default:
                return twoPlayerLayouts[Random.Range(0, twoPlayerLayouts.Length)];
        }
    }

    string[,] SplitRoomCSV()
    {
        string[] _roomRows = currentLayout.text.Split(lineSeperater);    // gets the number of rows
        string[] _roomColumns = _roomRows[0].Split(fieldSeperator);       // gets the number of columns

        string[,] roomRowsAndColumns = new string[_roomRows.Length, _roomColumns.Length]; // creates the 2d array of the correct size

        roomRows = roomRowsAndColumns.GetUpperBound(0);
        roomColumns = roomRowsAndColumns.GetUpperBound(1);

        for (int i = 0; i < roomRows; i++)
        {
            _roomColumns = _roomRows[i].Split(fieldSeperator);
            for (int j = 0; j < roomColumns; j++)
            {
                roomRowsAndColumns[i, j] = _roomColumns[j];  // populates the array
            }
        }
        return roomRowsAndColumns;
    }

    void GetAllRoomIndices(string[,] _roomLayout, ref List<string> _roomCoordinates, ref int _roomCount)
    {
        for (int i = 0; i < roomRows; i++)
        {
            for (int j = 0; j < roomColumns; j++)
            {
                if (_roomLayout[i,j] == "r")            // if this is a room index
                {
                    _roomCount++;
                    _roomCoordinates.Add(System.String.Format("{0},{1}", i, j));
                }
                else if (_roomLayout[i, j] == "pr")     // if it's the pod room
                {
                    podRoomRow = i;
                    podRoomColumn = j;
                }
                else if (_roomLayout[i, j] == "cr")     // if it's a crossroad
                {
                    crossroadCoordinates.Add(System.String.Format("{0},{1}", i, j));
                }
            }
        }

        totalRoomNumber = _roomCoordinates.Count;
    }

    void SpawnCrossroads()
    {
        if (crossroadCoordinates.Count == 0) return;
        
        string[] indices = new string[2];
        Vector3 pos = new Vector3();

        for (int i = crossroadCoordinates.Count - 1; i >= 0; i--)
        {
            indices = crossroadCoordinates[i].Split(fieldSeperator);
            pos.z = (podRoomRow - System.Int32.Parse(indices[0])) * 28;
            pos.x = (System.Int32.Parse(indices[1]) - podRoomColumn) * 28;
            RpcSpawnRoomOnClient(0, pos);

            crossroadCoordinates.RemoveAt(i);
        }
    }

    void SpawnMustHaves(ref List<string> _roomCoordinates)
    {
        GetMustHaveRooms();                             // find all the rooms that must be included at least once
        if (mustHaveRooms.Count == 0) return;

        int arrayPos;
        string[] indices = new string[2];
        Vector3 pos = new Vector3();

        foreach (int _roomIndex in mustHaveRooms)       // for each must-have room put it in a random room location
        {
            arrayPos = Random.Range(0, _roomCoordinates.Count);        // pick random position
            indices = _roomCoordinates[arrayPos].Split(fieldSeperator);    // get the string index
            pos.z = (podRoomRow - System.Int32.Parse(indices[0])) * 28;
            pos.x = (System.Int32.Parse(indices[1]) - podRoomColumn) * 28;
            CmdSpawnRoomOnClient(_roomIndex, pos);
            roomTypeCount.Add(allRooms[_roomIndex].roomName, 1);
            totalRoomCount++;

            _roomCoordinates.RemoveAt(arrayPos);    // remove the used room coordinate
        }
    }

    void GetMustHaveRooms()
    {
        for (int i = 0; i < allRooms.Length; i++)
        {
            if (allRooms[i].mustHaveOne)
            {
                mustHaveRooms.Add(i);
            }
        }
    }

    void SpawnAllRooms(ref List<string> _roomCoordinates)
    {
        if (_roomCoordinates.Count == 0) return;

        string[] indices = new string[2];

        Vector3 pos = new Vector3();

        for (int i = _roomCoordinates.Count - 1; i >= 0; i--)
        {
            int roomIndex = 0;

            // get random room
            for (int j = 0; j < 1; j++)
            {
                roomIndex = Random.Range(1, allRooms.Length);
                if (!CanAddRoom(allRooms[roomIndex]))   // if this room can't be added generate another room
                {
                    j = -1;
                }
            }
            //
            indices = _roomCoordinates[i].Split(fieldSeperator);
            pos.z = (podRoomRow - System.Int32.Parse(indices[0])) * 28;
            pos.x = (System.Int32.Parse(indices[1]) - podRoomColumn) * 28;

            CmdSpawnRoomOnClient(roomIndex, pos);
            
            _roomCoordinates.RemoveAt(i);
            totalRoomCount++;
        }
    }

    [Command]
    void CmdSpawnRoomOnClient(int roomIndex, Vector3 pos)
    {
        RpcSpawnRoomOnClient(roomIndex, pos);
    }

    [ClientRpc]
    void RpcSpawnRoomOnClient(int roomIndex, Vector3 pos)
    {
        foreach (GameObject player in allPlayers)
        {
            player.GetComponent<PCControl>().LML.SpawnRoom(roomIndex, pos);
        }
    }

    bool CanAddRoom(Room _room)                      // check that room number is within limits and update list of added rooms
    {
        if (roomTypeCount.ContainsKey(_room.roomName))  // if this room has been spawned before increment the value after checking limits
        {
            if (_room.limit != 0 && roomTypeCount[_room.roomName] >= _room.limit)   // if an explicit limit has been set and there are at least as many rooms of that type, do not spawn
            {
                return false;
            }
            if (_room.percentLimit != 0 && roomTypeCount[_room.roomName] / totalRoomNumber >= _room.percentLimit / 100) // if the percent limit has been reached, don't spawn
            {
                return false;
            }
            roomTypeCount[_room.roomName]++;
        }
        else                                        // else add this room to the room type counter
        {
            roomTypeCount.Add(_room.roomName, 1);
        }
        return true;
    }

    [ClientRpc]
    void RpcBuildNavmeshes()
    {
        // Build Navmeshes
        foreach (GameObject player in allPlayers)
        {
            player.GetComponent<PCControl>().LML.BuildNavmeshes();
        }
        print("all navmeshes built");
        //
    }

    #endregion

    void ReadPickupData(InventoryPickup.ItemType _type)
    {
        string[] pickupCSV = GetPickupCSV(_type);
        
        spawnList.Clear();
        foreach (string pickup in pickupCSV)
        {
            string[] fields = pickup.Split(fieldSeperator);
            if (fields.Length > 1)
            {
                PickupSpawner ps = new PickupSpawner(fields[0], int.Parse(fields[1]));
                spawnList.Add(ps);  // create an item to spawn with a name and rarity
            }
        }
    }

    string[] GetPickupCSV(InventoryPickup.ItemType _type)   // returns the CSV from which to get pickup names and their rarities
    {
        switch (_type)
        {
            case InventoryPickup.ItemType.tool:
                return toolCSV.text.Split(lineSeperater);
            case InventoryPickup.ItemType.component:
                return compCSV.text.Split(lineSeperater);
            case InventoryPickup.ItemType.boost:
                return boostCSV.text.Split(lineSeperater);
            default:
                return toolCSV.text.Split(lineSeperater);
        }
    }

    void SpawnPickups(InventoryPickup.ItemType _type)   // TODO spawn enough for double the players to be able to leave 
    {                                                   // e.g. 2 players requiring 3 tools and 6 comps, spawn 12 tools, 24 comps and 20 boosts
        ReadPickupData(_type);

        int numItemsRequired;
        switch (_type)      //set the number required for the item
        {
            case InventoryPickup.ItemType.tool:
                numItemsRequired = toolsRequired;
                break;
            case InventoryPickup.ItemType.component:
                numItemsRequired = compsRequired;
                break;
            case InventoryPickup.ItemType.boost:
                numItemsRequired = boostsPerPlayer;
                break;
        }

        List<InventoryPickup> pickupPool = new List<InventoryPickup>();   // the possible pool of spawned pickups
        foreach (PickupSpawner item in spawnList)
        {
            for (int i = 0; i < item.rarity; i++)       // rarity defines how many of this object go into the potential spawn pool
            {
                pickupPool.Add(new InventoryPickup(item.pickupName, _type, -1));
            }
        }

        // TODO Spawn the correct amount of pickups in the various containers and spawn points

        InventoryPickup[] toBeSpawned = new InventoryPickup[playerNumber * toolsRequired * itemMulitplier];    // this contains the pickups chosen from pickupPool to be spawned around the map

        for (int i = 0; i < toBeSpawned.Length; i++)
        {
            if (toBeSpawned[i] == null) // while there are still spaces in the array add Pickups
            {
                int index = Random.Range(0, pickupPool.Count - 1);
                InventoryPickup PU = new InventoryPickup(pickupPool[index].itemName, InventoryPickup.ItemType.tool, -1);
                toBeSpawned[i] = PU;
            }
        }

        for (int i = 0; i < toBeSpawned.Length; i++)    // spawn chosen tools. TODO disperse all pickups around level
        {
            var PU = Instantiate(pickupPrefab);

            //// is serial unique?
            for (int j = 0; j < 1; j++)
            {
                PU.serial = Random.Range(1000000, 9999999);
                if (serialsUsed.Count > 0 && serialsUsed[j] != 0)
                {
                    if (PU.serial == serialsUsed[j])
                    {
                        j = -1;
                    }
                }
            }
            ////

            //// add new serial to serialsUsed array
            for (int j = 0; j < serialsUsed.Count; j++)
            {
                serialsUsed.Add(PU.serial);
            }
            ////

            //// set world object's name, type and icon
            PU.pickupType = _type;
            PU.itemName = toBeSpawned[i].itemName;
            ////

            switch (_type)      //set the relevant hierarchy name for the item
            {
                case InventoryPickup.ItemType.tool:
                    PU.name = toBeSpawned[i].itemName + " - TOOL";
                    break;
                case InventoryPickup.ItemType.component:
                    PU.name = toBeSpawned[i].itemName + " - COMPONENT";
                    break;
                case InventoryPickup.ItemType.boost:
                    PU.name = toBeSpawned[i].itemName + " - BOOST";
                    break;
            }

            NetworkServer.Spawn(PU.gameObject);
        }

        // check enough unique ones (playerNumber * toolsRequired * 2)?
}

    //void SpawnTools()
    //{
    //    ReadPickupData();

    //    List<InventoryPickup> pickupPool = new List<InventoryPickup>();   // the possible pool of spawned pickups
    //    foreach (PickupSpawner item in spawnList)
    //    {
    //        for (int i = 0; i < item.rarity; i++)       // rarity defines how many of this object go into the potential spawn pool
    //        {
    //            pickupPool.Add(new InventoryPickup(item.pickupName, InventoryPickup.ItemType.tool, -1, toolIcon));
    //        }
    //    }

    //    // Spawn the correct amount of tools in the various containers and spawn points

    //    InventoryPickup[] toBeSpawned = new InventoryPickup[playerNumber * toolsRequired * 4];    // this contains the pickups chosen from pickupPool to be spawned around the map

    //    for (int i = 0; i < toBeSpawned.Length; i++)    
    //    {
    //        if (toBeSpawned[i] == null) // while there are still spaces in the array add Pickups
    //        {
    //            int index = Random.Range(0, pickupPool.Count - 1);
    //            InventoryPickup PU = new InventoryPickup(pickupPool[index].itemName, InventoryPickup.ItemType.tool, -1, toolIcon);
    //            toBeSpawned[i] = PU;
    //        }
    //    }

    //    int[] serialsUsed = new int[playerNumber * toolsRequired * 4];  // for checking that all pickups have unique serials

    //    for (int i = 0; i < toBeSpawned.Length; i++)    // spawn chosen tools
    //    {
    //        var PU = Instantiate(pickupPrefab);

    //        //// is serial unique?
    //        for (int j = 0; j < 1; j++)
    //        {
    //            PU.serial = Random.Range(1000000, 999999);
    //            if (serialsUsed[j] != 0)
    //            {
    //                if (PU.serial == serialsUsed[j])
    //                {
    //                    j = -1;
    //                    break;
    //                }
    //            }
    //        }
    //        ////

    //        //// add new serial to serialsUsed array
    //        for (int j = 0; j < serialsUsed.Length; j++)
    //        {
    //            if (serialsUsed[j] == 0) serialsUsed[j] = PU.serial;
    //        }
    //        ////

    //        PU.pickupType = InventoryPickup.ItemType.tool;


    //        switch (PU.pickupType)      //set the relevant icon for the item
    //        {
    //            case InventoryPickup.ItemType.tool:
    //                PU.icon = toolIcon;
    //                break;
    //            case InventoryPickup.ItemType.component:
    //                PU.icon = compIcon;
    //                break;
    //            case InventoryPickup.ItemType.boost:
    //                PU.icon = boostIcon;
    //                break;
    //            default:
    //                break;
    //        }
    //        PU.itemName = toBeSpawned[i].itemName;
    //        PU.name = toBeSpawned[i].itemName + " - TOOL";
    //        NetworkServer.Spawn(PU.gameObject);
    //    }

    //    //check enough unique ones (playerNumber * toolsRequired * 2)
    //}
}
