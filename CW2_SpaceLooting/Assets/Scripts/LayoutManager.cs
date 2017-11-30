using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// http://www.theappguruz.com/blog/unity-csv-parsing-unity used for reference

public class PickupSpawner
{
    public string pickupName;
    public int rarity;
}

public class prePickup
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

    public TextAsset toolCSV;           //names and rarity values of tools
    public List<PickupSpawner> spawnList = new List<PickupSpawner>();
    private char lineSeperater = '\n';
    private char fieldSeperator = ',';
    
    void Start()
    {
        if (!isServer)
        {
            return;
        }
        DontDestroyOnLoad(this);

        print("BEGIN");
        SpawnPickups();
    }


    void Update()
    {

    }

    void ReadToolData()
    {
        string[] tools = toolCSV.text.Split(lineSeperater);
        foreach (string tool in tools)
        {
            string[] fields = tool.Split(fieldSeperator);
            if (fields.Length > 1)
            {
                PickupSpawner ps = new PickupSpawner();
                ps.pickupName = fields[0];
                ps.rarity = int.Parse(fields[1]);
                spawnList.Add(ps);  //create an item to spawn with a name and rarity
            }
        }
    }

    void SpawnPickups() //TODO spawn enough for double the players to be able to leave 
    {                   //e.g. for 2 players requiring 3 tools and 6 comps spawn 12 tools, 24 comps and 20 boosts
        SpawnTools();
    }

    void SpawnTools()
    {
        ReadToolData();

        List<InventoryPickup> pickupPool = new List<InventoryPickup>();   //the possible pool of spawned pickups
        foreach (PickupSpawner item in spawnList)
        {
            for (int i = 0; i < item.rarity; i++)       //rarity defines how many of this object go into the potential spawn pool
            {
                InventoryPickup PU = new InventoryPickup(item.pickupName, InventoryPickup.ItemType.tool, -1);
                pickupPool.Add(PU);
            }
        }

        //Spawn the correct amount of tools in the various containers and spawn points

        InventoryPickup[] toBeSpawned = new InventoryPickup[playerNumber * toolsRequired * 4];    //this contains the chosen pickups to be spawned around the map

        for (int i = 0; i < toBeSpawned.Length; i++)    
        {
            if (toBeSpawned[i] == null) //while there are still spaces in the array add Pickups
            {
                int index = Random.Range(0, pickupPool.Count - 1);
                InventoryPickup PU = new InventoryPickup(pickupPool[index].itemName, InventoryPickup.ItemType.tool, -1);
                toBeSpawned[i] = PU;
            }
        }

        int[] serialsUsed = new int[playerNumber * toolsRequired * 4];

        for (int i = 0; i < toBeSpawned.Length; i++)    //spawn chosen tools
        {
            var PU = Instantiate(pickupPrefab);

            //// is serial unique?
            for (int j = 0; j < 1; j++)
            {
                PU.serial = Random.Range(1000000, 999999);
                if (serialsUsed[j] != 0)
                {
                    if (PU.serial == serialsUsed[j])
                    {
                        j = -1;
                        break;
                    }
                }
            }

            //bool repeated = false;
            //do
            //{
            //    PU.serial = Random.Range(1000000, 999999);
            //    for (int j = 0; j < serialsUsed.Length; j++)
            //    {
            //        if (serialsUsed[j] != 0)
            //        {
            //            if (PU.serial == serialsUsed[j])
            //            {
            //                repeated = true;
            //                break;
            //            }
            //        }
            //    }
            //    print(repeated);
            //} while (repeated);
            ////

            //// add new serial to serialsUsed array
            for (int j = 0; j < serialsUsed.Length; j++)
            {
                if (serialsUsed[j] == 0) serialsUsed[j] = PU.serial;
            }
            ////

            PU.pickupType = InventoryPickup.ItemType.tool;
            PU.itemName = toBeSpawned[i].itemName;
            PU.name = toBeSpawned[i].itemName + " - TOOL";
            NetworkServer.Spawn(PU.gameObject);
        }

        //check enough unique ones (playerNumber * toolsRequired * 2)
    }
}
