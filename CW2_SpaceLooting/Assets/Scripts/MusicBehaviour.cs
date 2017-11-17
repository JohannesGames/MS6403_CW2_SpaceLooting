using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBehaviour : MonoBehaviour
{
    public AudioSource[] songStems; //index 0 is start riff, 1 is build tension, 2 is final layer, 3 is drums
    private float stemLength;
    private float nextLoopTime; //when the song will loop again
    [Range(0, 1)]
    public float songVol = 0.5f;    //volume of playback

    void Start()
    {
        for (int i = 0; i < songStems.Length; i++)
        {
            songStems[i] = Instantiate(songStems[i], transform);
            songStems[i].volume = 0;
        }
        stemLength = songStems[0].clip.length;
        nextLoopTime = Time.time + stemLength;
    }

    void Update()
    {
        if (songStems[0].volume < songVol)
        {
            songStems[0].volume += Time.deltaTime / 100;
        }
        else
            songStems[0].volume = songVol;

        if (Time.time >= nextLoopTime - 0.6f)
        {
            ChangeMusic();
            nextLoopTime = Time.time + stemLength;
        }
    }

    void ChangeMusic()
    {
        if (songStems[3].volume < songVol)
        {
            songStems[3].volume += songVol / 2;
        }
        else
        {
            songStems[3].volume = songVol;
        }
    }
}
