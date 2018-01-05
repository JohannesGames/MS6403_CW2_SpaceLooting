﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickMessage
{
    public QuickMessage(string txt, float rTime)
    {
        displayMessage = txt;
        removeTime = rTime;
    }
    public string displayMessage = " ";
    public float removeTime;
}

public class HUDManager : MonoBehaviour
{


    // Inventory Panel
    public Button closeInventoryButton;
    public AudioSource closeInventorySFX;
    public Button openInventoryButton;
    public AudioSource openInventorySFX;
    public RectTransform inventoryPanel;
    public RectTransform inventoryListContent;  //where the inventory content is (displayed) childed
    public List<ListItemInventory> listOfItemsInInventory = new List<ListItemInventory>();
    public ListItemInventory listItem;
    public Sprite toolIcon;
    public Sprite compIcon;
    public Sprite boostIcon;
    //////////////

    // Single Item Panel
    public RectTransform singleItemPanel;   //the panel containing info for single items found in the world
    public Button closeSingleItemButton;
    public AudioSource closeSingleItemSFX;
    public AudioSource openSingleItemSFX;
    public SingleItemWorld sItemWorld;  //the UI prefab of single items found in the world
    //////////////

    // Container Panel
    [HideInInspector]
    public Container openContainer = null;
    public SingleItemWorld containerItem;
    public Button closeContainerPanel;
    public RectTransform containerPanel;
    public RectTransform containerListContent;
    /////////////

    // Repair Pod Panel
    public RectTransform repairPodPanel;
    [HideInInspector]
    public RepairPod rp;
    [HideInInspector]
    public bool inRepairPodScreen;

    /////////////

    // Quick Message Panel
    public RectTransform quickMessagePanel;
    public float displayTime = 3;
    public Text msgPrefab;
    List<QuickMessage> allCurrentMessages = new List<QuickMessage>();  //all messages currently displayed

    /////////////

    public float pickupThrowStrength;   //the strength with which pickups are thrown from the player when dropped

    public PCControl pc;   //the local PC
    public PCInventory pcInv;   //the local PC's inventory

    private bool menuActive;    //is a menu active

    void Start()
    {
        rp = GetComponent<RepairPod>();
        openInventoryButton.onClick.AddListener(OpenInventoryPanel);
        closeInventoryButton.onClick.AddListener(CloseInventory);
        closeSingleItemButton.onClick.AddListener(CloseSingleItem);
    }

    void Update()
    {
        CheckMessages();
    }


    void LateUpdate()
    {
        if (inventoryPanel.gameObject.activeSelf ||
            singleItemPanel.gameObject.activeSelf ||
            containerPanel.gameObject.activeSelf)   //in case a menu was closed but another panel remains open
            menuActive = true;

        pc.isInMenu = menuActive;
    }

    #region OPEN UI ELEMENTS

    public void OpenSingleItemPanel(Pickup sItem)
    {
        SingleItemWorld temp = Instantiate(sItemWorld, singleItemPanel);
        temp.itemButtonPickup.GetComponent<PickupItemButton>().hm = this;
        temp.itemData = new InventoryPickup(sItem);
        temp.itemInWorld = sItem.gameObject;

        switch (sItem.pickupType)
        {
            case InventoryPickup.ItemType.tool:
                temp.itemButtonConsume.gameObject.SetActive(false);     //only show Consume button for boosts
                temp.itemIcon.sprite = toolIcon;
                break;
            case InventoryPickup.ItemType.component:
                temp.itemButtonConsume.gameObject.SetActive(false);     //only show Consume button for boosts
                temp.itemIcon.sprite = compIcon;
                break;
            case InventoryPickup.ItemType.boost:
                temp.itemIcon.sprite = boostIcon;
                break;
        }

        singleItemPanel.gameObject.SetActive(true); //show panel
        closeSingleItemButton.gameObject.SetActive(true);   //show close inventory button
        openInventoryButton.gameObject.SetActive(false);
        pc.isInMenu = true; //stop other input
        menuActive = true;
        //openSingleItemSFX.Play();
    }

    public void OpenInventoryPanel()
    {
        UpdateInventory();

        inventoryPanel.gameObject.SetActive(true); //show panel
        closeInventoryButton.gameObject.SetActive(true);
        openInventoryButton.gameObject.SetActive(false);
        pc.isInMenu = true;
        menuActive = true;
        //openInventorySFX.Play();
    }

    public void OpenContainerPanel(Container con)
    {
        openContainer = con;
        UpdateContainer();

        containerPanel.gameObject.SetActive(true);
        OpenInventoryPanel();
    }

    public void OpenRepairPodPanel()
    {
        inRepairPodScreen = true;
        repairPodPanel.gameObject.SetActive(true);
        OpenInventoryPanel();
    }
    #endregion

    #region CLOSE UI ELEMENTS

    public void CloseInventory()
    {
        inventoryPanel.gameObject.SetActive(false);
        closeInventoryButton.gameObject.SetActive(false);
        ClearInventoryList();
        openInventoryButton.gameObject.SetActive(true);
        menuActive = false;
        //closeInventorySFX.Play();
        CloseContainerPanel();
        CloseRepairPodPanel();
    }

    public void CloseSingleItem()
    {
        singleItemPanel.gameObject.SetActive(false);
        closeSingleItemButton.gameObject.SetActive(false);
        openInventoryButton.gameObject.SetActive(true);
        if (singleItemPanel.childCount > 1)
        {
            int cCount = singleItemPanel.childCount;
            for (int i = cCount - 1; i > 0; i--)
                Destroy(singleItemPanel.GetChild(i).gameObject);    //delete any children created when populating the single item panel UI element
        }
        menuActive = false;
        //closeSingleItemSFX.Play();
    }

    public void CloseContainerPanel()
    {
        containerPanel.gameObject.SetActive(false);
        closeContainerPanel.gameObject.SetActive(false);
        openInventoryButton.gameObject.SetActive(true);
        ClearContainerList();
        menuActive = false;
        openContainer = null;
    }

    void CloseRepairPodPanel()
    {
        inRepairPodScreen = false;
        repairPodPanel.gameObject.SetActive(false);
        rp.ResetText();
        if (rp.inRepairScreen.Count > 0)
        {
            for (int i = rp.inRepairScreen.Count - 1; i >= 0; i--)
            {
                pcInv.inInventory.Add(rp.inRepairScreen[i]);
                rp.inRepairScreen.RemoveAt(i);
            }
            foreach (PodListItem item in rp.componentsRequired)
                item.itemInSlot = null;
            foreach (PodListItem item in rp.toolsRequired)
                item.itemInSlot = null;
        }
    }
    #endregion

    #region MOVE ITEMS

    public void DropInventoryItem(InventoryPickup iPick)
    {
        Vector3 tRot = new Vector3(30, Random.Range(0, 360), 0);    //generate random rotation to throw object
        Vector3 tPos = pc.transform.position + Vector3.up * 2;

        for (int i = 0; i < pcInv.inInventory.Count; i++)           // remove pickup from pc inventory
        {
            if (pcInv.inInventory[i].serial == iPick.serial)
            {
                pcInv.inInventory.RemoveAt(i);
                break;
            }
        }
        UpdateInventory();
        pc.CmdDropObject(new LayoutManager.ItemPickups(iPick));

        //for (int i = 0; i < pc.pcInvenTrans.childCount; i++)    //move from PC Inventory transform in hierarchy to main hierarchy with no parent
        //{
        //    if (pc.pcInvenTrans.GetChild(i).GetComponent<Pickup>().itemName == tItem.itemName)  //find item in PC hierarchy
        //    {
        //        Transform invTrans = pc.pcInvenTrans.GetChild(i);
        //        invTrans.position = tPos;
        //        invTrans.rotation = Quaternion.Euler(tRot);
        //        invTrans.GetComponent<Rigidbody>().isKinematic = false;
        //        invTrans.GetComponent<Pickup>().particleSys.Play();
        //        invTrans.GetComponent<Rigidbody>().AddForce(invTrans.TransformDirection(Vector3.up) * pickupThrowStrength);   //throw it a small distance next to the PC
        //        invTrans.parent = null;
        //        invTrans.GetComponent<Collider>().enabled = true;
        //        invTrans.GetComponent<MeshRenderer>().enabled = true;
        //    }
        //}

        //if (listOfItemsInInventory.Count > 0)    //then delete it from the hierarchy and the PC inventory list
        //{
        //    for (int i = 0; i < listOfItemsInInventory.Count; i++)   //remove from UI hierarchy
        //    {
        //        if (listOfItemsInInventory[i].itemData.itemName == tItem.itemName)
        //        {
        //            Destroy(listOfItemsInInventory[i].gameObject);   //remove from UI hierarchy
        //            listOfItemsInInventory.RemoveAt(i);
        //            for (int j = 0; j < pcInv.inInventory.Count; j++)   //remove from PCInventory list<>
        //            {
        //                if (pcInv.inInventory[j].itemName == tItem.itemName)
        //                    pcInv.inInventory.RemoveAt(j);  //remove from PCInventory list<>
        //            }
        //            break;
        //        }
        //    }
        //}
    }

    public void MoveToContainer(InventoryPickup tItem)
    {
        pc.AddObjectToContainer(tItem.serial, openContainer);
        UpdateContainer();
        UpdateInventory();
    }

    public void MoveFromContainer(InventoryPickup tItem)
    {
        //for (int i = 0; i < openContainer.inContainer.Count; i++)    // container to PC inventory
        //{
        //    if (openContainer.inContainer[i].serial == tItem.serial)
        //    {
        //        openContainer.inContainer.RemoveAt(i);
        //        pcInv.inInventory.Add(tItem);
        //    }
        //}
        UpdateContainer();
        UpdateInventory();
    }

    #endregion

    #region UPDATE UI LISTS

    public void UpdateContainer()
    {
        if (openContainer)
        {
            ClearContainerList();
            foreach (int puSerial in openContainer.inContainer)
            {
                SingleItemWorld temp = Instantiate(containerItem, containerListContent);
                temp.itemData = new InventoryPickup(LayoutManager.LM.GetItemFromSerial(puSerial));
                temp.itemButtonPickup.GetComponent<PickupItemButton>().hm = this;
                temp.isInContainer = true;

                switch (temp.itemData.pickupType)
                {
                    case InventoryPickup.ItemType.tool: // if it's a tool
                        temp.itemIcon.sprite = toolIcon;
                        break;
                    case InventoryPickup.ItemType.component: // if it's a component
                        temp.itemIcon.sprite = compIcon;
                        break;
                    case InventoryPickup.ItemType.boost: // if it's a boost
                        temp.itemIcon.sprite = boostIcon;
                        temp.itemButtonConsume.gameObject.SetActive(true);     //only show Consume button for boosts
                        break;
                }

            }
        }
    }

    public void UpdateInventory()
    {
        ClearInventoryList();
        foreach (InventoryPickup item in pcInv.inInventory)  //used foreach because its slightly neater
        {
            if (!inRepairPodScreen)
            {
                ListItemInventory temp = Instantiate(listItem, inventoryListContent);   //create new UI element in the inventory list
                temp.hm = this;
                switch (item.pickupType)
                {
                    case InventoryPickup.ItemType.tool:
                        temp.itemImage.sprite = toolIcon;
                        break;
                    case InventoryPickup.ItemType.component:
                        temp.itemImage.sprite = compIcon;
                        break;
                    case InventoryPickup.ItemType.boost:
                        temp.itemImage.sprite = boostIcon;
                        break;
                }
                listOfItemsInInventory.Add(temp);
                temp.itemData.itemName = item.itemName;
                temp.itemData.pickupType = item.pickupType;
                temp.serial = temp.itemData.serial = item.serial;
            }
            else    // if on repair screen
            {
                if (rp.CheckPickup(item))   // checks that the item (or one of the same name) is not already in use
                {
                    bool repeat = false;    // checks for repeated pickups in Inventory list (unique items required to repair pod)
                    if (inventoryListContent.childCount > 0)
                    {
                        for (int i = 0; i < inventoryListContent.childCount; i++)   // check currently displayed pickups. if one of these pickups is already shown do not repeat
                        {
                            if (item.itemName ==
                                inventoryListContent.GetChild(i).GetComponent<ListItemInventory>().itemData.itemName)
                                repeat = true;
                        }
                    }
                    if (!repeat)    // if no other of this pickup have been added to repair screen
                    {
                        ListItemInventory temp = Instantiate(listItem, inventoryListContent);   //create new UI element in the inventory list
                        temp.hm = this;
                        InventoryPickup tempData = temp.GetComponent<InventoryPickup>();
                        temp.hm = this;
                        switch (item.pickupType)
                        {
                            case InventoryPickup.ItemType.tool:
                                temp.itemImage.sprite = toolIcon;
                                break;
                            case InventoryPickup.ItemType.component:
                                temp.itemImage.sprite = compIcon;
                                break;
                            case InventoryPickup.ItemType.boost:
                                temp.itemImage.sprite = boostIcon;
                                break;
                        }
                        listOfItemsInInventory.Add(temp);
                        temp.itemData.itemName = item.itemName;
                        temp.itemData.pickupType = item.pickupType;
                        temp.serial = temp.itemData.serial = item.serial;
                    }
                }
            }
        }
    }

    void ClearContainerList()
    {
        foreach (SingleItemWorld item in containerListContent.GetComponentsInChildren<SingleItemWorld>())
        {
            Destroy(item.gameObject);
        }
    }

    void ClearInventoryList()
    {
        if (inventoryListContent.childCount > 0)
        {
            for (int i = inventoryListContent.childCount - 1; i >= 0; i--)
            {
                Destroy(inventoryListContent.GetChild(i).gameObject);    //delete any children created when populating the inventory list UI element
                inventoryListContent.GetChild(i).transform.SetParent(null);
            }
        }
        listOfItemsInInventory.Clear();
    }

    void CheckMessages()
    {
        for (int i = allCurrentMessages.Count - 1; i >= 0; i--)
        {
            if (Time.time >= allCurrentMessages[i].removeTime)
            {
                for (int j = 0; j < quickMessagePanel.childCount; j++)
                {
                    if (quickMessagePanel.GetChild(j)
                        && quickMessagePanel.GetChild(j).GetComponent<Text>().text == allCurrentMessages[i].displayMessage)
                    {
                        Destroy(quickMessagePanel.GetChild(j).gameObject);
                        break;
                    }
                }
                allCurrentMessages.Remove(allCurrentMessages[i]);
            }
        }
    }

    public void AddMessage(string msg, bool isPC)
    {
        QuickMessage qm = new QuickMessage(msg, Time.time + displayTime);
        allCurrentMessages.Add(qm);
        Text newMessage = Instantiate(msgPrefab, quickMessagePanel);
        newMessage.text = msg;
        if (isPC)
            newMessage.color = Color.white;
    }

    #endregion
}
