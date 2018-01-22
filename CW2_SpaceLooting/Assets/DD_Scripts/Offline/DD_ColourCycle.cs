using UnityEngine;
using System.Collections;

public class DD_ColourCycle : MonoBehaviour {

    public Color colorStart = Color.red;
    public Color colorEnd = Color.green;
    public float duration = 1.0F;
    public Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        rend.material.color = Color.Lerp(colorStart, colorEnd, lerp);
    }
}
