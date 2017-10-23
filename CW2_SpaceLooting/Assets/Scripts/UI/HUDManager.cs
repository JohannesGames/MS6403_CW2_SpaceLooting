using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Button closeInventoryButton;
    public RectTransform inventoryPanel;
    public RectTransform singleItemPanel;   //the panel containing info for single items found in the world
    public SingleItemWorld sItemWorld;  //the UI prefab of single items found in the world
    public RectTransform containerPanel;

    public GameObject[] allUIPanels;  //all the UI panels

    public PCControl pc;   //the local PC

    private bool menuActive;

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PCControl>();  //get the local PC here
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

    void ClearInventoryLists()
    {
        if (singleItemPanel.childCount > 1)
        {
            int cCount = singleItemPanel.childCount;
            for (int i = 1; i < cCount; i++)    
                Destroy(singleItemPanel.GetChild(i).gameObject);    //delete any children created when populating this UI element
        }
    }
}
