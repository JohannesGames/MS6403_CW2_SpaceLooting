using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public Button howToButton;
    public RectTransform howToPanel;
    public JB_NetworkManager nm;


    void Start()
    {
        hostButton.onClick.AddListener(BeginHost);
        joinButton.onClick.AddListener(JoinGame);
        howToButton.onClick.AddListener(OpenHowToPanel);
    }

    public void OpenHowToPanel()
    {
        howToPanel.gameObject.SetActive(true);
    }

    public void CloseHowToPanel()
    {
        howToPanel.gameObject.SetActive(false);
    }

    public void BeginHost()
    {
        nm.StartHost();
        nm.fader.StartFadeOut(1);
        gameObject.SetActive(false);
        nm.fader.StartFadeIn(1);
    }

    public void JoinGame()
    {
        nm.StartClient();
        nm.fader.StartFadeOut(1);
        gameObject.SetActive(false);
        nm.fader.StartFadeIn(1);
    }
}
