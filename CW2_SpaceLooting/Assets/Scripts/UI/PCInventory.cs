using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInventory : MonoBehaviour
{
    public List<InventoryPickup> inInventory = new List<InventoryPickup>();  //where all the PC's items are stored
    PCControl pc;

    void Start()
    {
        pc = GetComponent<PCControl>();
    }


    void Update()
    {
        CheckEncumbrance();
    }

    public void AddItemInventory(Pickup tItem)  //called by button in UI
    {
        //Pickup temp = Instantiate(tItem, pc.pcInventory.transform);
        switch (tItem.pickupType)
        {
            case Pickup.ItemType.tool:
                inInventory.Add(new InventoryPickup(tItem.itemName, InventoryPickup.ItemType.tool, tItem.serial));
                break;
            case Pickup.ItemType.component:
                inInventory.Add(new InventoryPickup(tItem.itemName, InventoryPickup.ItemType.component, tItem.serial));
                break;
            case Pickup.ItemType.boost:
                inInventory.Add(new InventoryPickup(tItem.itemName, InventoryPickup.ItemType.boost, tItem.serial));
                break;
            default:
                break;
        }
    }
    
    public void UpdateInventory()
    {
        inInventory.Clear();

        //foreach (Pickup item in pc.pcInvenTrans.GetComponentsInChildren<Pickup>())
        //{
        //    inInventory.Add(item);
        //}
    }

    void CheckEncumbrance()
    {
        int pickupsInInventory = 0;

        foreach (InventoryPickup item in inInventory)
        {
            if (item.pickupType == InventoryPickup.ItemType.component || item.pickupType == InventoryPickup.ItemType.tool)    //carrying a tool or a component adds to encumbrance
            {
                pickupsInInventory++;
            }
        }
        if (pickupsInInventory > 5)
            pickupsInInventory = 5;
        
        switch(pickupsInInventory)
        {
            case 5:
                pc.SetNavSpeed(0.5f);   //minimum speed is half of the normal speed
                break;
            case 4:
                pc.SetNavSpeed(0.8f);
                break;
            case 3:
                pc.SetNavSpeed(0.92f);
                break;
            default:
                pc.SetNavSpeed(1);  //otherwise use base speed
                break;
        }
    }
}
