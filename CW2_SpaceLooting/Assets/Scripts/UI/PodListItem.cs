using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PodListItem : MonoBehaviour
{
    public Pickup itemInSlot;
    public Button removeButton;
    public int listIndex = -1;
    RepairPod rp;
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
        if (itemInSlot.pickupType == Pickup.ItemType.component)
            type = 0;
        else
            type = 1;
        rp.RemoveItem(listIndex, type);
    }
}
