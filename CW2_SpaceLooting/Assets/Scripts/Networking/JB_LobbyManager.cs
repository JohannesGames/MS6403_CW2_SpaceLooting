using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JB_LobbyManager : NetworkLobbyManager
{
    public RectTransform startScreen;
    public RectTransform lobbyScreen;
    public Button backButton;
    private RectTransform currentPanel;

    ulong currentMatchNetworkID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickHost()
    {
        StartHost();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();

        print("Start host");
        backDelegate = BackButtonStopHostClbk;
        ChangeTo(lobbyScreen);
    }

    private void ChangeTo(RectTransform newPanel)
    {
        if (currentPanel)
        {
            newPanel.gameObject.SetActive(false);
        }

        if (newPanel)
        {
            currentPanel = newPanel;
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

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        print("Created match");
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        currentMatchNetworkID = (System.UInt64)matchInfo.networkId;

        if (!success)
        {
            print("Failed to create match");
        }
        else
        {
            print("Successfully created match: " + matchInfo.networkId);
        }
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
