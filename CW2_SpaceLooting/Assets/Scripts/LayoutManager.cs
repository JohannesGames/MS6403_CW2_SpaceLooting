using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    private char lineSeperater = '\n';
    private char fieldSeperator = ',';
    
    void Start()
    {
        if (!isServer)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        
        SpawnPickups(InventoryPickup.ItemType.tool);
        SpawnPickups(InventoryPickup.ItemType.component);

        //foreach (InventoryPickup.ItemType _type in System.Enum.GetValues(typeof(InventoryPickup.ItemType))) // iterate through all pickup types and spawn all the objects
        //{
        //    SpawnPickups(_type);
        //}
    }

    void ReadPickupData(InventoryPickup.ItemType _type)
    {
        string[] pickupCSV = GetPickupCSV(_type);

        //switch (_type)
        //{
        //    case InventoryPickup.ItemType.tool:
        //        pickupCSV = toolCSV.text.Split(lineSeperater);
        //        break;
        //    case InventoryPickup.ItemType.component:
        //        pickupCSV = compCSV.text.Split(lineSeperater);
        //        break;
        //    case InventoryPickup.ItemType.boost:
        //        pickupCSV = boostCSV.text.Split(lineSeperater);
        //        break;
        //}
        
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

        int[] serialsUsed = new int[playerNumber * toolsRequired * itemMulitplier];  // for checking that all pickups have unique serials

        for (int i = 0; i < toBeSpawned.Length; i++)    // spawn chosen tools. TODO disperse all pickups around level
        {
            var PU = Instantiate(pickupPrefab);

            //// is serial unique?
            for (int j = 0; j < 1; j++)
            {
                PU.serial = Random.Range(1000000, 9999999);
                if (serialsUsed[j] != 0)
                {
                    if (PU.serial == serialsUsed[j])
                    {
                        j = -1;
                        break;
                    }
                }
            }
            ////

            //// add new serial to serialsUsed array
            for (int j = 0; j < serialsUsed.Length; j++)
            {
                if (serialsUsed[j] == 0) serialsUsed[j] = PU.serial;
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
