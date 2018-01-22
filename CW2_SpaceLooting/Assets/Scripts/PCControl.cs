using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.UI;

public class PCControl : NetworkBehaviour
{

    public struct ItemPickups
    {
        public ItemPickups(string _name, int _type, int _serial)
        {
            itemName = _name;
            pickupType = _type;
            serial = _serial;
        }

        public ItemPickups(InventoryPickup ip)
        {
            itemName = ip.itemName;
            int typeIndex = (int)ip.pickupType;
            pickupType = typeIndex;
            serial = ip.serial;
        }

        public string itemName;

        public int pickupType;

        public int serial;
    }


    // For host
    List<GameObject> allPlayers = new List<GameObject>();
    [HideInInspector]
    [SyncVar]
    public bool isHost;
    [HideInInspector]
    [SyncVar]
    public bool isReady;
    [HideInInspector]
    [SyncVar(hook = "OnChangePlayerReady")]
    public int playersReady;
    [HideInInspector]
    [SyncVar]
    public int playerCount;
    //
    public GameObject playerSelectPrefab;
    private PlayerSelection playerSelectScreen;
    public bool isInMenu;
    public GameObject cameraContainer;
    public GameObject escapePod;
    public Light LI_Point;
    public float FL_Gravity;
    CameraFollow CF_Camera;
    Vector2 V2_FingerPosition;  //where the player presses the screen
    [HideInInspector]
    public NavMeshAgent NMA_PC;
    [HideInInspector]
    public LayoutManagerLocal LML;
    float speedNav; //the max navmeshagent speed of the PC
    CharacterController CC;
    PCInventory pcI;
    public HUDManager hM;
    public LocalGameManager lgm;
    public Transform podInventory;
    public GameObject GO_PickupNext = null;   //the object the PC is moving towards
    public List<Collider> CO_InRadius = new List<Collider>();
    public float FL_Reach = 1.5f;   //reach of PC
    public Transform pcInvenTrans;  //where the inventory is in the hierarchy
    public Pickup pickupPrefab;
    public Color outlineColour;
    public float pickupThrowStrength = 500;

    //////// Double click
    public float doubleClickInterval = 0.5f;
    float doubleClickTime;   //the time by which a second click must have been inputed
    bool instantPickup; //if double clicked, pick up instantly
                        /////////

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        LML = GetComponent<LayoutManagerLocal>();
        Destroy(Camera.main.gameObject);
        CameraAndOutline(true);
        NMA_PC = GetComponent<NavMeshAgent>();
        speedNav = NMA_PC.speed;
        CC = GetComponent<CharacterController>();
        var psTemp = Instantiate(playerSelectPrefab);
        playerSelectScreen = psTemp.GetComponent<PlayerSelection>();
        playerSelectScreen.pc = this;
        isInMenu = true;
        Instantiate(lgm);
        pcI = GetComponent<PCInventory>();
        CameraAndOutline(false);
        CmdSpawnEscapePod();
        GetComponent<AudioListener>().enabled = true;
        AudioListener.volume = 0;
        if (isServer)
        {
            isHost = true;
            playerCount = 1;
            allPlayers.Add(this.gameObject);
            playerSelectScreen.launchButton.GetComponentInChildren<Text>().text = "begin";
            playerSelectScreen.hostInfo.gameObject.SetActive(true);
            playerSelectScreen.hostInfo.text = "0 of 0 players ready";
        }
        else
        {
            Invoke("AddPlayerToGame", .1f);
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CheckForPickups();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!CC.isGrounded && NMA_PC.enabled) //if PC is not grounded move downwards
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - FL_Gravity, transform.position.z);
            transform.position = pos;
        }
        if (!isInMenu)  //if player is not using a menu take input
        {
            TakeInput();
        }
    }

    void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CO_InRadius.Clear();
    }

    void AddPlayerToGame()
    {
        if (isServer)
        {
            return;
        }
        CmdAddPlayerToGame(this.gameObject);
    }

    public void BeginGame()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!isServer)
        {
            isReady = true;
            CmdSetPlayerReady();
            return;
        }
        CmdStartGameOnAllClients();
        CmdBeginGameHost();
        var lm = FindObjectOfType<LayoutManager>();
        foreach (GameObject player in allPlayers)
        {
            lm.allPlayers.Add(player);
        }
        lm.BeginTheSpawning();
    }

    [Command]
    public void CmdBeginGameHost()
    {
        RpcClosePlayerSelectScreen();
    }

    [ClientRpc]
    public void RpcClosePlayerSelectScreen()
    {
        foreach (GameObject player in allPlayers)
        {
            player.GetComponent<PCControl>().playerSelectScreen.gameObject.SetActive(false);
        }
    }

    [Command]
    public void CmdSetPlayerReady()
    {
        playersReady++;
    }

    [Command]
    void CmdStartGameOnAllClients()
    {
        RpcBeginGameAllPlayers();
    }

    [ClientRpc]
    public void RpcBeginGameAllPlayers()
    {
        foreach (GameObject player in allPlayers)
        {
            var _pc = player.GetComponent<PCControl>();
            _pc.hM = Instantiate(_pc.hM);
            _pc.hM.pcInv = _pc.pcI;
            _pc.hM.pc = _pc;
        }
        // TODO set correct gameobject to PC
    }

    [Command]
    public void CmdAddPlayerToGame(GameObject _player)
    {
        if (!isServer && !isLocalPlayer)
        {
            return;
        }
        allPlayers.Add(_player);
        playerCount++;
        playerSelectScreen.hostInfo.text = playersReady + " of " + (playerCount - 1) + " players ready";
    }

    void OnChangePlayerReady(int readyNum)
    {
        if (!isServer)
        {
            return;
        }
        playerCount = allPlayers.Count;
        playerSelectScreen.hostInfo.text = readyNum + " of " + (playerCount - 1) + " players ready";
    }

    [Command]
    void CmdSpawnEscapePod()
    {
        // Spawn escape pod and set it to relevant layer
        escapePod = Instantiate(escapePod, transform.position, Quaternion.identity);
        NetworkServer.Spawn(escapePod);
        int podLayer = LayerMask.NameToLayer("Pod");
        escapePod.layer = podLayer;
        for (int i = 0; i < escapePod.transform.childCount; i++)
            escapePod.transform.GetChild(i).gameObject.layer = podLayer;
        //
    }

    void CameraAndOutline(bool isLocal)
    {
        if (isLocal)
        {
            CF_Camera = Instantiate(cameraContainer, transform.position, transform.rotation).GetComponent<CameraFollow>();
            CF_Camera.GO_PC = gameObject;
            Demo pcCameraOutlineScript = CF_Camera.pcCamera.GetComponent<Demo>();
            Outline pcO = GetComponentInChildren<Outline>();
            pcO.m_OutlineColor = outlineColour;
            pcCameraOutlineScript.m_Outlines[0] = pcO;
            pcCameraOutlineScript.OutlineApply(null, pcO.gameObject);
            pcCameraOutlineScript.m_PrevMouseOn = pcO.gameObject;

            CF_Camera.pcCamera.GetComponent<Demo>().m_Outlines = FindObjectsOfType<Outline>();
        }
    }

    #region // Input //

    void TakeInput()
    {
        if (Input.GetButtonDown("Fire1"))   //once finger hits screen take position
        {
            V2_FingerPosition = Input.mousePosition;
        }

        if (Input.GetButtonUp("Fire1"))     //once finger leaves screen move character to specified point
        {
            CheckInput();
        }
    }

    void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(V2_FingerPosition);
        int pickupIndex = LayerMask.NameToLayer("Pickup");
        int floorIndex = LayerMask.NameToLayer("Floor");
        int podIndex = LayerMask.NameToLayer("Pod");
        int containerIndex = LayerMask.NameToLayer("Container");    //only check "Pickup", "Floor", "Pod" and "Container" layers

        if (pickupIndex == -1 || floorIndex == -1 || containerIndex == -1 || podIndex == -1)
            Debug.LogError("Layers incorrectly set up");
        else
        {
            RaycastHit hit;
            int layermask = (1 << pickupIndex | 1 << floorIndex | 1 << containerIndex | 1 << podIndex);    //raycast to "Pickup", "Floor", "Pod" and "Container" only
            if (Physics.Raycast(ray, out hit, 100, layermask))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Pickup")
                    || hit.transform.gameObject.layer == LayerMask.NameToLayer("Container")
                    || hit.transform.gameObject.layer == LayerMask.NameToLayer("Pod"))   //if its a pickup or container or pod, make it the next item to interact with
                {
                    if (hit.transform.gameObject == GO_PickupNext)  //if the object had previously been selected AND a double click is detected, pick it up instantly when reached
                    {
                        if (Time.time < doubleClickTime)
                            instantPickup = true;
                    }
                    else
                    {
                        instantPickup = false;
                        doubleClickTime = Time.time + doubleClickInterval;
                    }
                    GO_PickupNext = hit.transform.gameObject;
                }
                else
                {
                    GO_PickupNext = null;
                    instantPickup = false;
                }
                NMA_PC.SetDestination(hit.point);
            }
        }

        if (!LI_Point.enabled)   //turn on light with first input
        {
            LI_Point.enabled = true;
            LI_Point.gameObject.SetActive(true);
            LI_Point.transform.parent.gameObject.SetActive(true);
        }
    }
    #endregion

    void CheckForPickups()  //check if PC is within reach of any pickups or containers
    {
        if (GO_PickupNext)
        {
            int layerIndex = LayerMask.NameToLayer("Pickup");   //check only for pickups
            if (layerIndex == -1)
                Debug.LogError("No layer called \"Pickup\"");
            int layermask = LayerMask.GetMask("Pickup", "Container", "Pod");    //1 << layerIndex;
            if (GO_PickupNext != null)
            {
                Collider[] allInRadius = Physics.OverlapSphere(transform.position, FL_Reach, layermask, QueryTriggerInteraction.Ignore);

                if (allInRadius.Length > 0)
                {
                    foreach (Collider item in allInRadius)
                        CO_InRadius.Add(item);  //add all nearby items to list
                }

                for (int i = 0; i < allInRadius.Length; i++)    // check all items in range
                {
                    if (allInRadius[i].gameObject == GO_PickupNext) //if the desired pickup or container is within reach, stop and show inventory screen
                    {
                        NMA_PC.ResetPath(); //stop the PC on the navmesh

                        if (GO_PickupNext.layer == LayerMask.NameToLayer("Pickup"))  //if it's a pickup display it in the inventory
                        {
                            if (instantPickup)  //if the item was double-clicked pick up instantly without menu
                            {
                                PickupObject(GO_PickupNext);
                                break;
                            }
                            else if (Time.time >= doubleClickTime)  //allows time for a double click for pickups in the vicinity
                            {
                                Pickup temp = GO_PickupNext.GetComponent<Pickup>();
                                hM.OpenSingleItemPanel(temp);  //send what kind of pickup it is to the HUD manager
                                GO_PickupNext = null;
                                break;
                            }
                        }
                        else if (GO_PickupNext.layer == LayerMask.NameToLayer("Container"))    //if it's a container display it in the inventory
                        {
                            Container temp = GO_PickupNext.GetComponent<Container>();
                            hM.OpenContainerPanel(temp);
                            GO_PickupNext = null;
                            break;
                        }
                        else    //if it's the pod, open repair screen
                        {
                            hM.OpenRepairPodPanel();
                            GO_PickupNext = null;
                            break;
                        }

                    }
                }
            }
        }
    }

    public void PickupObject(GameObject obj)    // this is called when picking up from the game world
    {
        Pickup toPickUp = obj.GetComponent<Pickup>();
        hM.pcInv.AddItemInventory(new InventoryPickup(toPickUp));
        CmdPickupObject(obj);
        hM.AddMessage("Picked up: " + toPickUp.itemName, true);
        GO_PickupNext = null;
    }

    #region Commands

    [Command]
    void CmdPickupObject(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    [Command]
    public void CmdDropObject(ItemPickups ip)
    {
        Vector3 tRot = new Vector3(30, Random.Range(0, 360), 0);    //generate random rotation to throw object
        Vector3 tPos = transform.position + Vector3.up * 2;

        Pickup pu = Instantiate(pickupPrefab, tPos, Quaternion.identity);
        pu.itemName = ip.itemName;
        int index = (int)ip.pickupType;
        pu.pickupType = (InventoryPickup.ItemType)index;
        pu.serial = ip.serial;
        pu.transform.position = tPos;
        pu.transform.rotation = Quaternion.Euler(tRot);
        pu.GetComponent<Rigidbody>().AddForce(pu.transform.TransformDirection(Vector3.up) * pickupThrowStrength);   //throw it a small distance next to the PC
        NetworkServer.Spawn(pu.gameObject);
    }

    [Command]
    public void CmdAccessContainer(GameObject _con)
    {
        Container tCon = _con.GetComponent<Container>();

        bool repeatCheck = false;
        for (int i = 0; i < tCon.playersAccessing.Count; i++)
        {
            if (GetInstanceID() == tCon.playersAccessing[i].GetInstanceID())
            {
                repeatCheck = true;
            }
        }
        if (!repeatCheck) tCon.playersAccessing.Add(this.gameObject);

        tCon.RpcAddPlayers(this.gameObject);
    }

    [Command]
    public void CmdExitContainer(GameObject _con)
    {
        Container tCon = _con.GetComponent<Container>();

        for (int i = 0; i < tCon.playersAccessing.Count; i++)
        {
            if (GetInstanceID() == tCon.playersAccessing[i].GetInstanceID())
            {
                tCon.playersAccessing.RemoveAt(i);
            }
        }

        tCon.RpcRemovePlayer(this.gameObject);
    }

    [Command]
    public void CmdAddItemToContainer(GameObject _con, ItemPickups ip)
    {
        Container tCon = _con.GetComponent<Container>();

        if (tCon)
        {
            for (int i = 0; i < tCon.inContainer.Count; i++)  // ensure no repeats
                if (tCon.inContainer[i].serial == ip.serial) return;

            tCon.inContainer.Add(ip);
        }
        else
        {
            Debug.LogError("Attempted to add to non-existent container");
        }

        tCon.RpcUpdateAllPlayers();
    }

    [Command]
    public void CmdRemoveItemFromContainer(GameObject _con, ItemPickups ip)
    {
        Container tCon = _con.GetComponent<Container>();

        if (tCon)
        {
            for (int i = 0; i < tCon.inContainer.Count; i++)
            {
                if (tCon.inContainer[i].serial == ip.serial)
                {
                    tCon.inContainer.RemoveAt(i);
                    tCon.RpcUpdateAllPlayers();
                    return;
                }
            }
            print("No such item in this container");
        }
        else
        {
            Debug.LogError("Attempted to remove from non-existent container");
        }
    }

    #endregion

    public void LaunchPod()
    {
        print("Launching Hooray!");
    }

    public void SetNavSpeed(float mod)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        NMA_PC.speed = speedNav * mod;
    }
}
