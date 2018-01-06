using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInventory : MonoBehaviour
{
    private List<InventoryPickup> inInventory = new List<InventoryPickup>();  //where all the PC's items are stored
    PCControl pc;

    void Start()
    {
        pc = GetComponent<PCControl>();
    }


    void Update()
    {
        CheckEncumbrance();
    }

    public List<InventoryPickup> GetInventory() { return inInventory; }

    public void AddItemInventory(InventoryPickup tItem)  //called by button in UI
    {
        if (!CheckForRepeats(tItem.serial)) // ensure no items are added more than once
        {
            inInventory.Add(new InventoryPickup(tItem));
        }
    }

    bool CheckForRepeats(int _serial)
    {
        for (int i = 0; i < inInventory.Count; i++)
        {
            if (inInventory[i].serial == _serial) return true;
        }
        return false;
    }

    public void RemoveItemInventory(int _serial)
    {
        for (int i = 0; i < inInventory.Count; i++)
        {
            if (inInventory[i].serial == _serial)
            {
                inInventory.RemoveAt(i);
                return;
            }
        }
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
