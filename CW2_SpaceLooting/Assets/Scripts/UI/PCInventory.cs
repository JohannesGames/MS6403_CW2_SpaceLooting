using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInventory : MonoBehaviour
{
    public List<Pickup> inInventory = new List<Pickup>();  //where all the PC's items are stored
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
        inInventory.Add(tItem);
    }

    void CheckEncumbrance()
    {
        int pickupsInInventory = 0;

        foreach (Pickup item in inInventory)
        {
            if (item.pickupType == Pickup.ItemType.component || item.pickupType == Pickup.ItemType.tool)    //carrying a tool or a component adds to encumbrance
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
