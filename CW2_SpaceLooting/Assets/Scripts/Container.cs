using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public Transform inventoryLocation;   //where the items in the container are in the hierarchy
    [HideInInspector]
    public List<Pickup> inContainer = new List<Pickup>();

    void Start()
    {
        
    }
    
    void Update()
    {

    }

    public void CheckForItems()    //populate the inContainer list with items
    {
        foreach (Pickup item in inventoryLocation.GetComponentsInChildren<Pickup>())
        {
            inContainer.Add(item);
        }
    }
}
