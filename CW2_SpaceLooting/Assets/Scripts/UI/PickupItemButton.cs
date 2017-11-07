using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupItemButton : MonoBehaviour
{
    HUDManager hm;
    Button thisButton;
    void Start()
    {
        hm = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(PickupItem);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickupItem()
    {
        //TODO check whether someone else has already taken it
        SingleItemWorld temp = GetComponentInParent<SingleItemWorld>();
        PCInventory pci = hm.pc.gameObject.GetComponent<PCInventory>();
        pci.AddItemInventory(temp.itemInWorld.GetComponent<Pickup>());   //add item to inventory using PCInventory script
        temp.itemInWorld.transform.parent = hm.pc.pcInvenTrans;    //add to PC Inventory in hierarchy
        //temp.itemInWorld.SetActive(false);
        temp.itemInWorld.transform.GetComponent<Collider>().enabled = false;
        temp.itemInWorld.transform.GetComponent<MeshRenderer>().enabled = false;
        Destroy(GetComponentInParent<SingleItemWorld>().gameObject);   //remove from UI list
        if (hm.openContainer) //if the object is being picked up from a container
            hm.UpdateInventory();
        hm.CloseSingleItem();
    }
}
