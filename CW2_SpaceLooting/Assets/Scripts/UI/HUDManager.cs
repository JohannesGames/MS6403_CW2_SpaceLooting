using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Button closeInventoryButton;
    public Button openInventoryButton;
    public RectTransform inventoryPanel;
    public RectTransform inventoryListContent;  //where the inventory content is (displayed) childed
    public ListItemInventory listItem;
    public RectTransform singleItemPanel;   //the panel containing info for single items found in the world
    public SingleItemWorld sItemWorld;  //the UI prefab of single items found in the world
    public RectTransform containerPanel;

    public float pickupThrowStrength = 1000;
    public Pickup prefPickup;   //prefab for spawning pickups

    public GameObject[] allUIPanels;  //all the UI panels

    public PCControl pc;   //the local PC
    public PCInventory pcInv;   //the local PC's inventory

    private bool menuActive;

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PCControl>();  //get the local PC here
        pcInv = pc.GetComponent<PCInventory>();
        openInventoryButton.onClick.AddListener(OpenInventoryPanel);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        pc.isInMenu = menuActive;
    }

    public void CloseInventory()
    {
        //set all possible open UI panels to inactive
        foreach (GameObject panel in allUIPanels)
        {
            panel.gameObject.SetActive(false);
        }
        ClearInventoryLists();
        openInventoryButton.gameObject.SetActive(true);
        menuActive = false;
    }

    public void OpenSingleItemPanel(Pickup sItem)
    {
        SingleItemWorld temp = Instantiate(sItemWorld, singleItemPanel);
        temp.itemData = sItem;
        temp.itemInWorld = sItem.gameObject;

        if (sItem.pickupType != Pickup.ItemType.boost)  //only show Consume button for boosts
            temp.itemButtonConsume.gameObject.SetActive(false);

        singleItemPanel.gameObject.SetActive(true); //show panel
        closeInventoryButton.gameObject.SetActive(true);
        pc.isInMenu = true;
        menuActive = true;
    }

    public void OpenInventoryPanel()
    {
        foreach (Pickup item in pcInv.inInventory)
        {
            ListItemInventory temp = Instantiate(listItem, inventoryListContent);
            temp.itemData = item;
        }

        inventoryPanel.gameObject.SetActive(true); //show panel
        closeInventoryButton.gameObject.SetActive(true);
        openInventoryButton.gameObject.SetActive(false);
        pc.isInMenu = true;
        menuActive = true;
    }

    public void DropInventoryItem(Pickup tItem)
    {
        Vector3 tRot = new Vector3(30, Random.Range(0, 360), 0);    //generate random rotation to throw object
        Vector3 tPos = pc.transform.position + Vector3.up * 2;
        
        for (int i = 0; i < pc.pcInventory.transform.childCount; i++)    //move from PC "Inventory" empty gameobject in hierarchy to main hierarchy with no parent
        {
            if (pc.pcInventory.transform.GetChild(i).GetComponent<Pickup>().itemName == tItem.itemName)
            {
                Transform invTrans = pc.pcInventory.transform.GetChild(i);
                invTrans.position = tPos;
                invTrans.rotation = Quaternion.Euler(tRot);
                invTrans.GetComponent<MeshRenderer>().enabled = true;
                invTrans.GetComponent<Collider>().enabled = true;
                invTrans.GetComponent<Rigidbody>().AddForce(invTrans.TransformDirection(Vector3.up) * pickupThrowStrength);   //throw it a small distance next to the PC
                invTrans.parent = null;
            }
        }

//        Pickup temp = Instantiate(prefPickup, tPos, Quaternion.Euler(tRot));
//        temp.itemName = tItem.itemName;
//        temp.pickupType = tItem.pickupType;

//#region Name object in hierarchy
//        string iName = " ";   //name in hierarchy
//        switch(temp.pickupType)
//        {
//            case Pickup.ItemType.boost:
//                iName = "Boost: ";
//                break;
//            case Pickup.ItemType.component:
//                iName = "Comp: ";
//                break;
//            case Pickup.ItemType.tool:
//                iName = "Tool: ";
//                break;
//        }
//        temp.name = iName + temp.itemName + " - Pickup";
//        #endregion

//        temp.GetComponent<Rigidbody>().AddForce(temp.transform.TransformDirection(Vector3.up) * pickupThrowStrength);   //throw it a small distance next to the PC

        if (inventoryListContent.childCount > 0)    //then delete it from the hierarchy and the PC inventory list
        {
            for (int i = 0; i < inventoryListContent.childCount; i++)   //remove from UI hierarchy
            {
                if (inventoryListContent.GetChild(i).GetComponent<Pickup>().itemName == tItem.itemName)
                {
                    Destroy(inventoryListContent.GetChild(i).gameObject);   //remove from UI hierarchy
                    for (int j = 0; j < pcInv.inInventory.Count; j++)   //remove from PCInventory list<>
                    {
                        if (pcInv.inInventory[j].itemName == tItem.itemName)
                            pcInv.inInventory.RemoveAt(j);  //remove from PCInventory list<>
                    }
                    break;
                }
            }
        }
    }

    void ClearInventoryLists()
    {
        if (singleItemPanel.childCount > 1)
        {
            int cCount = singleItemPanel.childCount;
            for (int i = cCount - 1; i > 0; i--)    
                Destroy(singleItemPanel.GetChild(i).gameObject);    //delete any children created when populating the single item panel UI element
        }

        if (inventoryListContent.childCount > 0)
        {
            int cCount = inventoryListContent.childCount;
            for (int i = cCount - 1; i >= 0; i--)
                Destroy(inventoryListContent.GetChild(i).gameObject);    //delete any children created when populating the inventory list UI element
        }
    }
}
