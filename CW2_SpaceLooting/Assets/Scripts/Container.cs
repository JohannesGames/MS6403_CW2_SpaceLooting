using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public Transform inventoryLocation;   //where the items in the container are in the hierarchy
    [HideInInspector]
    public List<InventoryPickup> inContainer = new List<InventoryPickup>();

    void Start()
    {
        
    }
    
    void Update()
    {

    }

    public void AddItemContainer(InventoryPickup tItem)
    {
        inContainer.Add(tItem);
    }
}
