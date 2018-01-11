using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JB_LobbyPlayer : NetworkLobbyPlayer
{

    // Client UI elements
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
    public bool isHosting;

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

        if (isServer)
        {
            print("Host started game lobby");

            CmdStartHosting();

            JB_LobbyList.instance.AddPlayer(this);
        }
        else
        {
            print("Player entered lobby");

            JB_LobbyList.instance.AddPlayer(this);
        }
    }

    [Command]
    void CmdStartHosting()
    {
        isHosting = true;
    }

    public override void OnClientReady(bool readyState)
    {
        if (readyState)
        {

        }
    }

    public void OnReadyClicked()
    {
        print("ready");
        CmdSetReady();
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
