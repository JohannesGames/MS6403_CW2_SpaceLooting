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
        hm.pc.PickupObject(temp.itemInWorld);
        Destroy(GetComponentInParent<SingleItemWorld>().gameObject);   //remove from UI list
        if (hm.openContainer) //if the object is being picked up from a container
            hm.UpdateInventory();
        hm.CloseSingleItem();
    }
}
