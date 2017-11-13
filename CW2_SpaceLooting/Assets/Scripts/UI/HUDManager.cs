using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUDManager : MonoBehaviour
{
    public Image blurLayer;

    // Inventory Panel
    public Button closeInventoryButton;
    public AudioSource closeInventorySFX;
    public Button openInventoryButton;
    public AudioSource openInventorySFX;
    public RectTransform inventoryPanel;
    public RectTransform inventoryListContent;  //where the inventory content is (displayed) childed
    public List<ListItemInventory> listOfItemsInInventory = new List<ListItemInventory>();
    public ListItemInventory listItem;
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

    public float pickupThrowStrength;   //the strength with which pickups are thrown from the player when dropped

    public PCControl pc;   //the local PC
    public PCInventory pcInv;   //the local PC's inventory

    private bool menuActive;    //is a menu active

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PCControl>();  //get the local PC here
        pcInv = pc.GetComponent<PCInventory>();
        rp = GetComponent<RepairPod>();
        openInventoryButton.onClick.AddListener(OpenInventoryPanel);
    }


    void LateUpdate()
    {
        if (inventoryPanel.gameObject.activeSelf ||
            singleItemPanel.gameObject.activeSelf ||
            containerPanel.gameObject.activeSelf)   //in case a menu was closed but another panel remains open
            menuActive = true;

        pc.isInMenu = menuActive;
        blurLayer.gameObject.SetActive(menuActive);
    }

    #region OPEN UI ELEMENTS

    public void OpenSingleItemPanel(Pickup sItem)
    {
        SingleItemWorld temp = Instantiate(sItemWorld, singleItemPanel);
        temp.itemData = sItem;
        temp.itemInWorld = sItem.gameObject;

        if (sItem.pickupType != Pickup.ItemType.boost)  //only show Consume button for boosts
            temp.itemButtonConsume.gameObject.SetActive(false);

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
        con.inContainer.Clear();
        con.CheckForItems();
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
        if (rp.itemInventory.childCount > 0)
        {
            foreach (Pickup item in rp.itemInventory.GetComponentsInChildren<Pickup>())
                item.transform.parent = pc.pcInvenTrans;
            foreach (PodListItem item in rp.componentsRequired)
                item.itemInSlot = null;
            foreach (PodListItem item in rp.toolsRequired)
                item.itemInSlot = null;
        }
    }
    #endregion

    #region MOVE ITEMS

    public void DropInventoryItem(Pickup tItem)
    {
        Vector3 tRot = new Vector3(30, Random.Range(0, 360), 0);    //generate random rotation to throw object
        Vector3 tPos = pc.transform.position + Vector3.up * 2;

        for (int i = 0; i < pc.pcInvenTrans.childCount; i++)    //move from PC Inventory transform in hierarchy to main hierarchy with no parent
        {
            if (pc.pcInvenTrans.GetChild(i).GetComponent<Pickup>().itemName == tItem.itemName)
            {
                Transform invTrans = pc.pcInvenTrans.GetChild(i);
                invTrans.position = tPos;
                invTrans.rotation = Quaternion.Euler(tRot);
                invTrans.gameObject.SetActive(true);
                invTrans.GetComponent<Rigidbody>().AddForce(invTrans.TransformDirection(Vector3.up) * pickupThrowStrength);   //throw it a small distance next to the PC
                invTrans.parent = null;
                invTrans.GetComponent<Collider>().enabled = true;
                invTrans.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        if (listOfItemsInInventory.Count > 0)    //then delete it from the hierarchy and the PC inventory list
        {
            for (int i = 0; i < listOfItemsInInventory.Count; i++)   //remove from UI hierarchy
            {
                if (listOfItemsInInventory[i].itemData.itemName == tItem.itemName)
                {
                    Destroy(listOfItemsInInventory[i].gameObject);   //remove from UI hierarchy
                    listOfItemsInInventory.RemoveAt(i);
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

    public void MoveToContainer(Pickup tItem)
    {
        for (int i = 0; i < pc.pcInvenTrans.childCount; i++)    //move from PC Inventory transform in hierarchy to the container's inventory
        {
            if (pc.pcInvenTrans.GetChild(i).GetComponent<Pickup>().itemName == tItem.itemName)
            {
                pc.pcInvenTrans.GetChild(i).parent = openContainer.inventoryLocation;
            }
        }
        UpdateContainer();

        if (listOfItemsInInventory.Count > 0)    //delete it from the hierarchy and the PC inventory list
        {
            for (int i = 0; i < listOfItemsInInventory.Count; i++)   //remove from UI hierarchy
            {
                if (listOfItemsInInventory[i].itemData.itemName == tItem.itemName)
                {
                    Destroy(listOfItemsInInventory[i].gameObject);   //remove from UI hierarchy
                    listOfItemsInInventory.RemoveAt(i);
                    for (int j = 0; j < pcInv.inInventory.Count; j++)   //remove from PCInventory list<>
                    {
                        if (pcInv.inInventory[j].itemName == tItem.itemName)
                            pcInv.inInventory.RemoveAt(j);  //remove from PCInventory list<>
                    }
                    return;
                }
            }
        }
    }

    #endregion

    #region UPDATE UI LISTS

    public void UpdateContainer()
    {
        if (openContainer)
        {
            ClearContainerList();
            openContainer.CheckForItems();
            foreach (Pickup item in openContainer.inContainer)
            {
                SingleItemWorld temp = Instantiate(containerItem, containerListContent);

                if (item.pickupType != Pickup.ItemType.boost)  //only show Consume button for boosts
                    temp.itemButtonConsume.gameObject.SetActive(false);

                temp.itemData = item;
                temp.itemInWorld = item.gameObject;
            }
        }
    }

    public void UpdateInventory()
    {
        ClearInventoryList();
        pcInv.UpdateInventory();
        int tIndex = 0;
        foreach (Pickup item in pcInv.inInventory)  //used foreach because its prettier
        {
            if (!inRepairPodScreen)
            {
                ListItemInventory temp = Instantiate(listItem, inventoryListContent);   //create new UI element in the inventory list
                listOfItemsInInventory.Add(temp);
                listOfItemsInInventory[tIndex].itemData = item;
                tIndex++;
            }
            else    //if on repair screen
            {
                if (rp.CheckPickup(item))   //checks that the item (or one of the same name) is not already in use
                {
                    bool repeat = false;    //checks for repeated pickups in Inventory list (unique items required to repair pod)
                    if (inventoryListContent.childCount > 0)
                    {
                        for (int i = 0; i < inventoryListContent.childCount; i++)   //check currently displayed pickups. if one is already shown do not repeat
                        {
                            if (item.itemName ==
                                inventoryListContent.GetChild(i).GetComponent<ListItemInventory>().itemData.itemName)
                                repeat = true;
                        }
                    }
                    if (!repeat)
                    {
                        ListItemInventory temp = Instantiate(listItem, inventoryListContent);   //create new UI element in the inventory list
                        listOfItemsInInventory.Add(temp);
                        listOfItemsInInventory[tIndex].itemData = item;
                        tIndex++;
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
        if (openContainer)
            openContainer.inContainer.Clear();
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

    #endregion
}
