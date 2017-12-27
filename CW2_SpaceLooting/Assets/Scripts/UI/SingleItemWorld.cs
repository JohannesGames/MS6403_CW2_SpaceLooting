using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleItemWorld : MonoBehaviour
{
    public InventoryPickup itemData = new InventoryPickup();
    public Image itemIcon;
    public Text itemName;
    public Button itemButtonPickup;
    public Button itemButtonConsume;
    public GameObject itemInWorld;  //reference to item in the game world
    public bool isInContainer;
    
    void Start()
    {
        //update item icon
        itemName.text = itemData.itemName;
    }
    
    void Update()
    {

    }
}
