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
        SingleItemWorld temp = GetComponentInParent<SingleItemWorld>();
        PCInventory pci = hm.pc.gameObject.GetComponent<PCInventory>();
        pci.AddItemInventory(temp.itemData);   //add item to inventory using PCInventory script
        Destroy(GetComponentInParent<SingleItemWorld>().itemInWorld);
        hm.CloseInventory();
        Destroy(temp.gameObject);
    }
}
