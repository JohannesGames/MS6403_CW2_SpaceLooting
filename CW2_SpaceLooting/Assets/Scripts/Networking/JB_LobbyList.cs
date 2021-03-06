﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

// Based heavily on the Unity-provided lobby manager asset

public class JB_LobbyList : MonoBehaviour
{
    public static JB_LobbyList instance;

    public RectTransform playerList;
    private VerticalLayoutGroup layout;
    private List<JB_LobbyPlayer> allPlayers = new List<JB_LobbyPlayer>();

    void OnEnable()
    {
        instance = this;
        layout = playerList.GetComponent<VerticalLayoutGroup>();
    }

    void Update()
    {
        if (layout)
            layout.childAlignment = Time.frameCount % 2 == 0
                ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
    }

    public void AddPlayer(JB_LobbyPlayer player)
    {
        print("Lobby List: Adding Players");

        if (allPlayers.Contains(player))
            return;

        allPlayers.Add(player);

        player.transform.SetParent(playerList, false);

        //JB_LobbyManager.instance.playerCount.text = allPlayers.Count.ToString();

        //if (player.isServer)
        //{
        //    player.hostImage.gameObject.SetActive(true);
        //    player.clientImage.gameObject.SetActive(false);
        //    player.hostNameInput.gameObject.SetActive(true);
        //    player.clientName.gameObject.SetActive(false);
        //    player.playerReadyState.gameObject.SetActive(false);
        //    player.hostAcceptName.gameObject.SetActive(true);
        //    player.hostLaunchButton.transform.parent.gameObject.SetActive(true);
        //}

        //else
        //{
        //    print("Lobby List: Client Joined List");
        //}
    }
}