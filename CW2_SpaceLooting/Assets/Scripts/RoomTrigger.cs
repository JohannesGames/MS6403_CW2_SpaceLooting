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
        if (col.gameObject.layer == 8)  //is it the player?
        {
            if (RoomType == Rooms.PodRoom)  //if player enters pod room
            {
                
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == 8)  //is it the player
        {
            if (RoomType == Rooms.PodRoom)  //if player leaves pod room
            {
                
            }
        }
    }
}
