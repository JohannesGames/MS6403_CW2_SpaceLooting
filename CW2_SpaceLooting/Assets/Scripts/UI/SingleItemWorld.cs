using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleItemWorld : MonoBehaviour
{
    public Pickup itemData;
    public Image itemImage;
    public Text itemName;
    public Button itemButtonPickup;
    public Button itemButtonConsume;
    public GameObject itemInWorld;  //reference to item in the game world
    
    void Start()
    {
        //update item icon
        itemName.text = itemData.itemName;
    }
    
    void Update()
    {

    }
}
