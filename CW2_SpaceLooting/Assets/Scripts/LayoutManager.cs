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

        List<prePickup> pickupPool = new List<prePickup>();   //the possible pool of spawned pickups
        foreach (PickupSpawner item in spawnList)
        {
            for (int i = 0; i < item.rarity; i++)       //rarity defines how many of this object go into the potential spawn pool
            {
                prePickup PU = new prePickup();
                PU.itemName = item.pickupName;
                PU.pickupType = prePickup.ItemType.tool;
                pickupPool.Add(PU);
            }
        }

        //Spawn the correct amount of tools in the various containers and spawn points

        prePickup[] toBeSpawned = new prePickup[playerNumber * toolsRequired * 4];    //this contains the chosen pickups to be spawned around the map

        for (int i = 0; i < toBeSpawned.Length; i++)    
        {
            if (toBeSpawned[i] == null) //while there are still spaces in the array add Pickups
            {
                int index = Random.Range(0, pickupPool.Count - 1);
                prePickup PU = new prePickup();
                PU.itemName = pickupPool[index].itemName;
                PU.pickupType = prePickup.ItemType.tool;
                toBeSpawned[i] = PU;
            }
        }

        for (int i = 0; i < toBeSpawned.Length; i++)    //spawn chosen tools
        {
            var PU = Instantiate(pickupPrefab);
            PU.pickupType = Pickup.ItemType.tool;
            PU.itemName = toBeSpawned[i].itemName;
            PU.name = toBeSpawned[i].itemName + " - TOOL";
            NetworkServer.Spawn(PU.gameObject);
        }

        //check enough unique ones (playerNumber * toolsRequired * 2)
    }
}
