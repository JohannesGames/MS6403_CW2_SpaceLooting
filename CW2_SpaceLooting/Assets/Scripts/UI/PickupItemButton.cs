using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupItemButton : MonoBehaviour
{
    public HUDManager hm;
    Button thisButton;
    void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(PickupItem);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickupItem()
    {
        //TODO check whether someone else has already taken it

        SingleItemWorld temp = GetComponentInParent<SingleItemWorld>();

        if (temp.isInContainer)
        {
            hm.MoveFromContainer(temp.itemData);
            hm.AddMessage("Picked up: " + temp.itemData.itemName, true);
            Destroy(GetComponentInParent<SingleItemWorld>().gameObject);   //remove from UI list
        }
        else
        {
            hm.pc.PickupObject(temp.itemInWorld);
            Destroy(GetComponentInParent<SingleItemWorld>().gameObject);   //remove from UI list
            hm.CloseSingleItem();
        }
    }
}
