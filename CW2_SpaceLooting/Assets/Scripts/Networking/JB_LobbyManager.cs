using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.Match;

public class JB_LobbyManager : NetworkLobbyManager
{
    public static JB_LobbyManager instance;

    public RectTransform startScreen;
    public RectTransform lobbyScreen;
    public RectTransform clientScreen;
    public RectTransform hostScreen;
    public RectTransform playerSelectScreen;
    public GameObject selectableModels;
    public Button backButton;
    private RectTransform currentPanel;

    //ulong currentMatchNetworkID;

    string currentIP = "127.0.0.1";

    protected Prototype.NetworkLobby.LobbyHook lobbyHooks;

    // Use this for initialization
    void Start()
    {
        instance = this;
        currentPanel = startScreen;
        networkAddress = currentIP;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickHost()
    {
        StartHost();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        
        backDelegate = BackButtonStopHostClbk;
        ChangeTo(lobbyScreen);
        hostScreen.gameObject.SetActive(true);
    }

    public void OnClickJoin()
    {
        ChangeTo(lobbyScreen);
        clientScreen.gameObject.SetActive(true);
        StartClient();
    }

    private void ChangeTo(RectTransform newPanel)
    {
        if (currentPanel)
        {
            currentPanel.gameObject.SetActive(false);
        }

        if (newPanel)
        {
            currentPanel = newPanel;
            currentPanel.gameObject.SetActive(true);
        }

        if (currentPanel != startScreen)    // if we're not at the main menu
        {
            backButton.gameObject.SetActive(true);
        }
        else
        {
            backButton.gameObject.SetActive(false);
        }
    }

    //public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    //{
    //    print("Created match");
    //    base.OnMatchCreate(success, extendedInfo, matchInfo);
    //    currentMatchNetworkID = (System.UInt64)matchInfo.networkId;

    //    if (!success)
    //    {
    //        print("Failed to create match");
    //    }
    //    else
    //    {
    //        print("Successfully created match: " + matchInfo.networkId);
    //    }
    //}

    // When ready to go
    public override void OnLobbyServerPlayersReady()
    {
        bool allready = true;
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            if (lobbySlots[i] != null)
                allready &= lobbySlots[i].readyToBegin;
        }

        if (allready)   // if everyone is ready allow host to press LAUNCH
        {
            print("all ready");
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                JB_LobbyPlayer p = lobbySlots[i] as JB_LobbyPlayer;
                if (p)
                {
                    p.LaunchButtonAccessibility(true);
                }
            }
        }
    }

    public void ChangeToPlayScene()
    {
        ServerChangeScene(playScene);

        StartCoroutine(FindNetworkManager());

        startScreen.transform.parent.gameObject.SetActive(false);

        //ChangeTo(playerSelectScreen);
        //selectableModels = Instantiate(selectableModels);
    }

    IEnumerator FindNetworkManager()
    {
        NetworkManager newNM = null;

        while (!newNM)
        {
            newNM = FindObjectOfType<NetworkManager>();
            print("looking");
            yield return null;
        }

        print("found it!");
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        return true;
    }

    #region Back Button
    public delegate void BackButtonDelegate();
    public BackButtonDelegate backDelegate;
    public void GoBackButton()
    {
        backDelegate();
    }

    public void BackButtonClbk()
    {
        ChangeTo(startScreen);
    }

    public void BackButtonStopHostClbk()
    {
        StopHost();
        ChangeTo(startScreen);
    }

    public void BackButtonStopClientClbk()
    {
        StopClient();
        ChangeTo(startScreen);
    }
    #endregion
}
