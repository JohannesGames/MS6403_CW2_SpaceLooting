using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PodListItem : MonoBehaviour
{
    public InventoryPickup itemInSlot;
    public Button removeButton;
    public int listIndex = -1;
    public RepairPod rp;
    public Image icon;
    public InventoryPickup.ItemType podItemType;
    // Use this for initialization
    void Start()
    {
        removeButton.onClick.AddListener(RemoveFromSlot);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RemoveFromSlot()
    {
        if (itemInSlot != null)
        {
            rp.RemoveItem(listIndex, podItemType);
            rp.GetComponent<HUDManager>().UpdateInventory();
        }
    }
}
