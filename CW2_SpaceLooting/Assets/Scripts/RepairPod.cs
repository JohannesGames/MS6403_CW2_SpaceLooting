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
    public Transform itemInventory;  //where the items are kept in the hierarchy
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
        }

        for (int i = 0; i < toolsRequired.Length; i++)
        {
            toolsRequired[i] = Instantiate(itemSlot, toolsPanel);
            toolsRequired[i].listIndex = i;
        }
    }
    
    void Update()
    {
        
    }

    public void AddItem(Pickup tItem)
    {
        if (tItem.pickupType == Pickup.ItemType.component)  //if it's a component add it to the array
        {
            for (int i = 0; i < componentsRequired.Length; i++)
            {
                if (componentsRequired[i] == null)  //if there's space add it to the array and the pod in the hierarchy
                {
                    tItem.transform.parent = itemInventory;
                    componentsRequired[i].itemInSlot = tItem;
                    componentsRequired[i].removeButton.GetComponentInChildren<Text>().text = tItem.itemName;
                }
            }
        }
        else    //if it's a tool add it to the relevant array
        {
            for (int i = 0; i < toolsRequired.Length; i++)
            {
                if (toolsRequired[i] == null)  //if there's space add it to the array and the pod in the hierarchy
                {
                    tItem.transform.parent = itemInventory;
                    toolsRequired[i].itemInSlot = tItem;
                    toolsRequired[i].removeButton.GetComponentInChildren<Text>().text = tItem.itemName;
                }
            }
        }
    }

    public void RemoveItem(int index, int type) //type = 1 means tool, = 0 means component
    {
        if (type == 0)  //if it's a component
        {
            componentsRequired[index].itemInSlot.transform.parent = hm.pc.pcInvenTrans;
            componentsRequired[index].itemInSlot = null;
        }
        else
        {
            toolsRequired[index].itemInSlot.transform.parent = hm.pc.pcInvenTrans;
            toolsRequired[index].itemInSlot = null;
        }
    }

    public bool CheckPickup(Pickup tItem)
    {
        if (tItem.pickupType == Pickup.ItemType.component)  //if it's a component
        {
            for (int i = 0; i < componentsRequired.Length; i++)
            {
                if (componentsRequired[i])
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
                if (toolsRequired[i])
                {
                    if (toolsRequired[i].itemInSlot.itemName == tItem.itemName)  //if it's the same name the item is not needed and does not need to be displayed
                        return false;
                }
            }
        }

        return true;    //if it hasn't been found in the relevant array the item can be displayed
    }
}
