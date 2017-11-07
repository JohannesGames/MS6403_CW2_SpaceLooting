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
    private HUDManager hm;

    void Start()
    {
        hm = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();
        itemName.text = itemData.itemName;
        if (itemData.pickupType != Pickup.ItemType.boost)   //if its not a consumeable hide the "consume" button
            itemButtonConsume.gameObject.SetActive(false);

        if (hm.openContainer) //if a container is open, give option to put the item inside
            itemButtonDrop.gameObject.SetActive(false);
        else
            putInContainer.gameObject.SetActive(false);

        itemButtonDrop.onClick.AddListener(DropItem);           //called when player presses "drop item" button
        putInContainer.onClick.AddListener(ItemInContainer);    //called when player presses "move to container" button
    }

    public void DropItem()
    {
        hm.DropInventoryItem(itemData);
    }

    public void ItemInContainer()
    {
        hm.MoveToContainer(itemData);
    }
}
