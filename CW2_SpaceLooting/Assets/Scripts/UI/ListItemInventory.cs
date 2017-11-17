using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItemInventory : MonoBehaviour
{
    public Pickup itemData;
    public Image itemImage;
    public Text itemName;
    public Button itemButtonDrop;
    public Button itemButtonConsume;
    public Button putInContainer;
    public Button repairWithItem;
    public HUDManager hm;

    void Start()
    {
        itemName.text = itemData.itemName;
        if (itemData.pickupType != Pickup.ItemType.boost)   //if its not a consumeable hide the "consume" button
            itemButtonConsume.gameObject.SetActive(false);

        ShowButtons();

        itemButtonDrop.onClick.AddListener(DropItem);           //called when player presses "drop item" button
        putInContainer.onClick.AddListener(ItemInContainer);    //called when player presses "move to container" button
        repairWithItem.onClick.AddListener(AddToRepairSlot);    //called when player presses "repair" button
    }

    void ShowButtons()
    {
        if (hm.openContainer) //if a container is open, give option to put the item inside
        {
            itemButtonDrop.gameObject.SetActive(false);
            repairWithItem.gameObject.SetActive(false);
        }
        else if (hm.inRepairPodScreen)   //if in the repair screen hide the "use" and "drop buttons
        {
            itemButtonConsume.gameObject.SetActive(false);
            itemButtonDrop.gameObject.SetActive(false);
            putInContainer.gameObject.SetActive(false);
        }
        else    //if neither in a container or repair screen hide "store" and "repair" buttons
        {
            putInContainer.gameObject.SetActive(false);
            repairWithItem.gameObject.SetActive(false);
        }
    }

    public void DropItem()
    {
        hm.DropInventoryItem(itemData);
    }

    public void ItemInContainer()
    {
        hm.MoveToContainer(itemData);
    }

    public void AddToRepairSlot()
    {
        hm.rp.AddItem(itemData);
        hm.UpdateInventory();
    }
}
