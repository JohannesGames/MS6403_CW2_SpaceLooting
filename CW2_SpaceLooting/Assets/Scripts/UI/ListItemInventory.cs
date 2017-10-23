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
    private HUDManager hm;

    void Start()
    {
        hm = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();
        itemName.text = itemData.itemName;
        if (itemData.pickupType != Pickup.ItemType.boost)   //if its not a consumeable hide the "consume" button
            itemButtonConsume.gameObject.SetActive(false);

        itemButtonDrop.onClick.AddListener(DropItem);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DropItem()
    {
        hm.DropInventoryItem(itemData);
    }
}
