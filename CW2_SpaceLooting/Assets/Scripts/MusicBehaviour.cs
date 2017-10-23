using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBehaviour : MonoBehaviour
{
    public AudioSource[] songStems; //index 0 is start riff, 1 is build tension, 2 is final layer, 3 is drums
    private float stemLength;
    private float nextLoopTime; //when the song will loop again
    [Range(0,1)]
    public float songVol = 0.5f;    //volume of playback

    void Start()
    {
        for (int i = 0; i < songStems.Length; i++)
        {
            songStems[i] = Instantiate(songStems[i], transform);
            if (i > 0)
                songStems[i].volume = 0;
            else
            {
                songStems[i].volume = songVol;
                stemLength = songStems[i].clip.length;
            }
        }

        nextLoopTime = Time.time + stemLength;
    }

    void Update()
    {
        if (Time.time >= nextLoopTime)
        {
            ChangeMusic();
            nextLoopTime = Time.time + stemLength;
        }
    }

    void ChangeMusic()
    {
        songStems[3].volume = songVol;
    }
}
