﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairPod : MonoBehaviour
{
    public PodListItem[] componentsRequired; //the components required [count set in inspector]
    public RectTransform compsPanelTop;
    public RectTransform compsPanelBottom;
    public PodListItem[] toolsRequired;  //the tools required [count set in inspector]
    public RectTransform toolsPanel;
    public List<InventoryPickup> inRepairScreen = new List<InventoryPickup>();
    public PodListItem itemSlot;
    HUDManager hm;

    void Start()
    {
        hm = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();
        AddItemSlots();
    }

    void AddItemSlots()
    {
        for (int i = 0; i < componentsRequired.Length; i++) //populate the panel with the relevant amount of elements
        {
            if (i < componentsRequired.Length / 2)
            {
                componentsRequired[i] = Instantiate(itemSlot, compsPanelTop);
            }
            else
            {
                componentsRequired[i] = Instantiate(itemSlot, compsPanelBottom);
            }
            componentsRequired[i].listIndex = i;
            componentsRequired[i].podItemType = InventoryPickup.ItemType.component;
        }

        for (int i = 0; i < toolsRequired.Length; i++)
        {
            toolsRequired[i] = Instantiate(itemSlot, toolsPanel);
            toolsRequired[i].listIndex = i;
            toolsRequired[i].podItemType = InventoryPickup.ItemType.tool;
        }
    }

    void Update()
    {

    }

    public void AddItem(InventoryPickup tItem)
    {
        if (tItem.pickupType == InventoryPickup.ItemType.component)  //if it's a component add it to the array
        {
            for (int i = 0; i < componentsRequired.Length; i++)
            {
                if (componentsRequired[i].itemInSlot == null)  //if there's space add it to the array and the pod in the hierarchy
                {
                    inRepairScreen.Add(new InventoryPickup(tItem));
                    hm.pcInv.inInventory.Remove(tItem);
                    componentsRequired[i].itemInSlot = tItem;
                    componentsRequired[i].removeButton.GetComponentInChildren<Text>().text = tItem.itemName;    // display item name in repair UI TODO make prettier
                    return;
                }
            }
        }
        else    //if it's a tool add it to the relevant array
        {
            for (int i = 0; i < toolsRequired.Length; i++)
            {
                if (toolsRequired[i].itemInSlot == null)  //if there's space add it to the array and the pod in the hierarchy
                {
                    inRepairScreen.Add(new InventoryPickup(tItem));
                    hm.pcInv.inInventory.Remove(tItem);
                    toolsRequired[i].itemInSlot = tItem;
                    toolsRequired[i].removeButton.GetComponentInChildren<Text>().text = tItem.itemName;
                    return;
                }
            }
        }
    }

    public void RemoveItem(int index, InventoryPickup.ItemType tType) //type = 1 means tool, = 0 means component
    {
        switch (tType)
        {
            case InventoryPickup.ItemType.tool:
                hm.pcInv.inInventory.Add(new InventoryPickup(toolsRequired[index].itemInSlot));
                toolsRequired[index].itemInSlot = null;
                toolsRequired[index].removeButton.GetComponentInChildren<Text>().text = "n/a";
                break;
            case InventoryPickup.ItemType.component:
                hm.pcInv.inInventory.Add(new InventoryPickup(componentsRequired[index].itemInSlot));
                componentsRequired[index].itemInSlot = null;
                componentsRequired[index].removeButton.GetComponentInChildren<Text>().text = "n/a";
                break;
            default:
                break;
        }
    }

    public void ResetText() //reset the text on the item slots in the repair screen
    {
        for (int i = 0; i < componentsRequired.Length; i++)
            componentsRequired[i].removeButton.GetComponentInChildren<Text>().text = "n/a";
        for (int i = 0; i < toolsRequired.Length; i++)
            toolsRequired[i].removeButton.GetComponentInChildren<Text>().text = "n/a";
    }

    public bool CheckPickup(InventoryPickup tItem)
    {
        if (tItem.pickupType == InventoryPickup.ItemType.component)  //if it's a component
        {
            for (int i = 0; i < componentsRequired.Length; i++)
            {
                if (componentsRequired[i].itemInSlot != null)
                {
                    if (componentsRequired[i].itemInSlot.itemName == tItem.itemName)  //if it's the same name the item is not needed and does not need to be displayed
                        return false;
                }
            }
        }
        else    //if it's a tool
        {
            for (int i = 0; i < toolsRequired.Length; i++)
            {
                if (toolsRequired[i].itemInSlot != null)
                {
                    if (toolsRequired[i].itemInSlot.itemName == tItem.itemName)  //if it's the same name the item is not needed and does not need to be displayed
                        return false;
                }
            }
        }

        return true;    //if it hasn't been found in the relevant array the item can be displayed
    }
}
