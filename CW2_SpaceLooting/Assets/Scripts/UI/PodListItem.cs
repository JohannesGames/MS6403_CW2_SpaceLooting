using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PodListItem : MonoBehaviour
{
    public InventoryPickup itemInSlot;
    public Button removeButton;
    public int listIndex = -1;
    RepairPod rp;
    public Image icon;
    // Use this for initialization
    void Start()
    {
        rp = GameObject.FindGameObjectWithTag("GameController").GetComponent<RepairPod>();
        removeButton.onClick.AddListener(RemoveFromSlot);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RemoveFromSlot()
    {
        int type;
        if (itemInSlot == null)
        {
            if (itemInSlot.pickupType == InventoryPickup.ItemType.component)
                type = 0;
            else
                type = 1;
            rp.RemoveItem(listIndex, type);
            rp.GetComponent<HUDManager>().UpdateInventory();
        }
    }
}
