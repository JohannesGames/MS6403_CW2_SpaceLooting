using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JB_LobbyPlayer : NetworkLobbyPlayer
{
    // Client UI elements
    private ClientScreen clientScreen;
    public InputField nameInput;
    public Button readyButton;
    public Text playerCount;

    // Host UI elements
    public Image hostImage;
    public InputField hostNameInput;
    public RectTransform hostLaunchButtonBlocker;
    public Button hostAcceptName;
    public Button hostLaunchButton;
    public Image clientImage;
    public Text clientName;
    public Image playerReadyState;
    public Sprite playerNotReady;
    public Sprite playerReadyUp;

    public string playerName = "";

    [SyncVar]
    public bool isReady;

    [SyncVar]
    public bool isHosting;

    void Start()
    {
        hostAcceptName.onClick.AddListener(OnReadyClicked);
        hostLaunchButton.onClick.AddListener(LaunchGame);
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        if (!isClient)
        {
            print("Lobby Player: Host Started Game Lobby");

            //CmdStartHosting();

            JB_LobbyList.instance.AddPlayer(this);
        }

        else
        {
            print("Lobby Player: Player Entered Lobby");

            JB_LobbyList.instance.AddPlayer(this);
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        isHosting = true;
        if (!NetworkServer.active && !clientScreen)
        {
            ClientScreen go = Resources.Load("Client Panel") as ClientScreen;
            clientScreen = Instantiate(go, JB_LobbyManager.instance.lobbyScreen);

            clientScreen.readyButton.onClick.AddListener(OnReadyClicked);
            UpdatePlayerInfo();
        }
        else
        {
            hostImage.gameObject.SetActive(true);
            clientImage.gameObject.SetActive(false);
            hostNameInput.gameObject.SetActive(true);
            clientName.gameObject.SetActive(false);
            playerReadyState.gameObject.SetActive(false);
            hostAcceptName.gameObject.SetActive(true);
            hostLaunchButton.transform.parent.gameObject.SetActive(true);
        }
    }

    public void UpdatePlayerInfo()
    {
        if (clientScreen)
        {
            int count = 0;
            for (int i = 0; i < JB_LobbyManager.instance.lobbySlots.Length; i++)
            {
                if (JB_LobbyManager.instance.lobbySlots[i])
                {
                    count++;
                }
            }
            clientScreen.playerCount.text = count.ToString();
        }
        else
        {
            if (isReady)
            {
                playerReadyState.sprite = playerReadyUp;
            }
            else
            {
                playerReadyState.sprite = playerNotReady;
            }

            clientName.text = nameInput.text;
        }
    }

    //[Command]
    //void CmdStartHosting()
    //{
    //    isHosting = true;
    //}

    public void OnReadyClicked()
    {
        print("Lobby Player: Ready");
        //CmdSetReady();
        SendReadyToBeginMessage();
    }

    [Command]
    void CmdSetReady()
    {
        isReady = !isReady;
    }

    public void LaunchButtonAccessibility(bool readiness)
    {
        hostLaunchButtonBlocker.gameObject.SetActive(!readiness);
    }

    public void LaunchGame()
    {
        JB_LobbyManager.instance.ChangeToPlayScene();
    }
}