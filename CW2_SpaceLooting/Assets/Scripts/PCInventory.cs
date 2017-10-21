using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInventory : MonoBehaviour
{
    List<Pickup> inInventory = new List<Pickup>();  //where all the PC's items are stored
    PCControl pc;

    void Start()
    {
        pc = GetComponent<PCControl>();
    }


    void Update()
    {
        CheckEncumbrance();
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

        if (pickupsInInventory >= 5)
        {
            pc.SetNavSpeed(0.2f);   //minimum speed = 0.2 times normal speed
        }
        else if (pickupsInInventory > 2)
        {
            pc.SetNavSpeed(1 / pickupsInInventory);
        }
    }
}
