using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Fading : MonoBehaviour
{
    public RectTransform fadePanel;
    private Image panelImage;

    public float defaultFadeInTime = 2;
    public float defaultFadeOutTime = 2;
    
	void Start () {
        panelImage = fadePanel.GetComponent<Image>();
        StartFadeIn();
	}

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn(defaultFadeInTime));
    }

    public void StartFadeIn(float length)
    {
        StartCoroutine(FadeIn(length));
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut(defaultFadeInTime));
    }

    public void StartFadeOut(float length)
    {
        StartCoroutine(FadeOut(length));
    }

    IEnumerator FadeIn(float length)
    {
        float timer = 0;
        float progress = 0;

        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / length;

            panelImage.color = Color.Lerp(Color.black, Color.clear, progress);

            yield return null;
        }
    }

    IEnumerator FadeOut(float length)
    {
        float timer = 0;
        float progress = 0;

        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / length;

            panelImage.color = Color.Lerp(Color.clear, Color.black, progress);

            yield return null;
        }
    }
}
