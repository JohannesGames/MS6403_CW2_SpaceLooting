using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public enum Rooms
    {
        Corridor,
        PodRoom
    };

    public Rooms RoomType;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 8)
        {
            if (RoomType == Rooms.PodRoom)  //if player enters pod room turn off light
            {
                col.GetComponent<PCControl>().LI_Point.enabled = false;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == 8)
        {
            if (RoomType == Rooms.PodRoom)  //if player leaves pod room turn on light
            {
                col.GetComponent<PCControl>().LI_Point.enabled = true;
            }
        }
    }
}
