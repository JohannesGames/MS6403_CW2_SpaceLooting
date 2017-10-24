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
        pci.AddItemInventory(temp.itemData);   //add item to inventory using PCInventory script
        temp.itemInWorld.transform.parent = hm.pc.transform;    //add to PC Inventory in hierarchy
        temp.itemInWorld.GetComponent<MeshRenderer>().enabled = false;
        temp.itemInWorld.GetComponent<Collider>().enabled = false;
        Destroy(GetComponentInParent<SingleItemWorld>().itemInWorld);   //remove from UI list
        hm.CloseInventory();
    }
}
