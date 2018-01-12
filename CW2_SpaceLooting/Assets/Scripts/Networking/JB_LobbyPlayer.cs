using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JB_LobbyPlayer : NetworkLobbyPlayer
{
    // Client UI elements
    public ClientScreen clientScreen;
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

            if (localPlayerAuthority)
            {
                clientScreen = Instantiate(clientScreen, JB_LobbyList.instance.transform);
                clientScreen.readyButton.onClick.AddListener(OnReadyClicked);
                JB_LobbyManager.instance.playerCount = clientScreen.playerCount;
            }

            JB_LobbyList.instance.AddPlayer(this);
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