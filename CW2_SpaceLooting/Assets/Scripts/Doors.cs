using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public GameObject doorSlide;    //part of the door that slides
    public bool isOpen;
    private bool doorIsMoving = false;
    private bool doorIsOpening;
    private bool doorIsClosing;
    public float countToClose = 3;  //count til door closes
    private float timeToClose;  //time when door is closed
    public float moveTime = 0.5f;   //how long the door lerp takes
    private float openProgress; //the 't' in the opening lerp
    private float closeProgress = 1;    //the 't' in the closing lerp
    private float openPosY = 1.2f;   //the chosen local y coordinate for the door's open position (has to be y as the door is rotated by 90)
    private Vector3 openPos;
    private float openScaleY = 0.01f;    //the chosen y scale for the open position
    private Vector3 openScale;
    private Vector3 closePos;
    private Vector3 closeScale;
    private AudioSource doorSound;

    void Start()
    {
        doorSlide.GetComponent<MeshRenderer>().enabled = true;
        closePos = doorSlide.transform.localPosition;
        openPos = new Vector3(closePos.x, openPosY, closePos.z);
        closeScale = doorSlide.transform.localScale;
        openScale = new Vector3(closeScale.x, openScaleY, closeScale.z);
        doorSound = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (isOpen && Time.time >= timeToClose)   //is it time to close
        {
            doorIsMoving = true;
            doorIsClosing = true;
            doorIsOpening = false;
        }

        if (doorIsMoving)
        {
            if (!doorSound.isPlaying)
                doorSound.Play();

            if (isOpen && doorIsClosing) //if it is closing
            {
                CloseLerp();
            }
            else if(doorIsOpening)    //if it is opening
            {
                OpenLerp();
            }
        }
        else
        {
            if (doorSound.isPlaying)
                doorSound.Stop();
        }
    }

    void OpenLerp()
    {
        float timeLeft = ((1 - openProgress) * moveTime) - Time.deltaTime;  //time remaining
        if (timeLeft > 0)   //if there's time left in the lerp
        {
            openProgress = 1 - (timeLeft / moveTime);
        }
        else    //else it is open
        {
            openProgress = 1;
            isOpen = true;  //door is now open
            doorIsMoving = false;   //door no longer needs to move
            doorIsOpening = false;
        }

        doorSlide.transform.localPosition = Vector3.Lerp(closePos, openPos, openProgress);
        doorSlide.transform.localScale = Vector3.Lerp(closeScale, openScale, openProgress);

        closeProgress = 1 - openProgress;
    }

    void CloseLerp()
    {
        float timeLeft = ((1 - closeProgress) * moveTime) - Time.deltaTime;  //time remaining
        if (timeLeft > 0)   //if there's time left in the lerp
        {
            closeProgress = 1 - (timeLeft / moveTime);
        }
        else    //else it is closed
        {
            closeProgress = 1;
            isOpen = false;  //door is now closed
            doorIsMoving = false;   //door no longer needs to move
            doorIsOpening = false;
        }

        doorSlide.transform.localPosition = Vector3.Lerp(openPos, closePos, closeProgress);
        doorSlide.transform.localScale = Vector3.Lerp(openScale, closeScale, closeProgress);

        openProgress = 1 - closeProgress;
    }

    void OnTriggerEnter()   //when a PC enters the door trigger open door
    {
        if (!isOpen)
        {
            doorIsMoving = true;
            doorIsOpening = true;
            doorIsClosing = false;
            timeToClose = Time.time + countToClose;
        }
    }

    void OnTriggerStay()    //while a PC is inside the door trigger reset countdown to close
    {
        timeToClose = Time.time + countToClose;
        doorIsOpening = true;
        doorIsClosing = false;
    }
}
