#if ENABLE_UNET
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking
{
    public enum PlayerSpawnMethod
    {
        Random,
        RoundRobin
    };

    [AddComponentMenu("Network/NetworkManager")]
    public class JB_NetworkManager : MonoBehaviour
    {
        // configuration
        [SerializeField] int m_NetworkPort = 7777;
        [SerializeField] bool m_ServerBindToIP;
        [SerializeField] string m_ServerBindAddress = "";
        [SerializeField] string m_NetworkAddress = "localhost";
        [SerializeField] bool m_DontDestroyOnLoad = true;
        [SerializeField] bool m_RunInBackground = true;
        [SerializeField] bool m_ScriptCRCCheck = true;
        [SerializeField] bool m_SendPeerInfo;
        [SerializeField] float m_MaxDelay = 0.01f;
        [SerializeField] LogFilter.FilterLevel m_LogLevel = (LogFilter.FilterLevel)LogFilter.Info;
        [SerializeField] GameObject m_PlayerPrefab;
        [SerializeField] bool m_AutoCreatePlayer = true;
        [SerializeField] PlayerSpawnMethod m_PlayerSpawnMethod;
        [SerializeField] string m_OfflineScene = "";
        [SerializeField] string m_OnlineScene = "";
        [SerializeField] List<GameObject> m_SpawnPrefabs = new List<GameObject>();

        [SerializeField] bool m_CustomConfig;
        [SerializeField] int m_MaxConnections = 4;
        [SerializeField] ConnectionConfig m_ConnectionConfig;
        [SerializeField] List<QosType> m_Channels = new List<QosType>();

        [SerializeField] bool m_UseWebSockets;
        [SerializeField] bool m_UseSimulator;
        [SerializeField] int m_SimulatedLatency = 1;
        [SerializeField] float m_PacketLossPercentage;

        // matchmaking configuration
        [SerializeField] string m_MatchHost = "mm.unet.unity3d.com";
        [SerializeField] int m_MatchPort = 443;

        private EndPoint m_EndPoint;

        // properties
        public int networkPort { get { return m_NetworkPort; } set { m_NetworkPort = value; } }
        public bool serverBindToIP { get { return m_ServerBindToIP; } set { m_ServerBindToIP = value; } }
        public string serverBindAddress { get { return m_ServerBindAddress; } set { m_ServerBindAddress = value; } }
        public string networkAddress { get { return m_NetworkAddress; } set { m_NetworkAddress = value; } }
        public bool dontDestroyOnLoad { get { return m_DontDestroyOnLoad; } set { m_DontDestroyOnLoad = value; } }
        public bool runInBackground { get { return m_RunInBackground; } set { m_RunInBackground = value; } }
        public bool scriptCRCCheck { get { return m_ScriptCRCCheck; } set { m_ScriptCRCCheck = value; } }
        public bool sendPeerInfo { get { return m_SendPeerInfo; } set { m_SendPeerInfo = value; } }
        public float maxDelay { get { return m_MaxDelay; } set { m_MaxDelay = value; } }
        public LogFilter.FilterLevel logLevel { get { return m_LogLevel; } set { m_LogLevel = value; LogFilter.currentLogLevel = (int)value; } }
        public GameObject playerPrefab { get { return m_PlayerPrefab; } set { m_PlayerPrefab = value; } }
        public bool autoCreatePlayer { get { return m_AutoCreatePlayer; } set { m_AutoCreatePlayer = value; } }
        public PlayerSpawnMethod playerSpawnMethod { get { return m_PlayerSpawnMethod; } set { m_PlayerSpawnMethod = value; } }
        public string offlineScene { get { return m_OfflineScene; } set { m_OfflineScene = value; } }
        public string onlineScene { get { return m_OnlineScene; } set { m_OnlineScene = value; } }
        public List<GameObject> spawnPrefabs { get { return m_SpawnPrefabs; } }
        public List<Transform> startPositions { get { return s_StartPositions; } }
        public bool customConfig { get { return m_CustomConfig; } set { m_CustomConfig = value; } }
        public ConnectionConfig connectionConfig { get { if (m_ConnectionConfig == null) { m_ConnectionConfig = new ConnectionConfig(); } return m_ConnectionConfig; } }
        public int maxConnections { get { return m_MaxConnections; } set { m_MaxConnections = value; } }
        public List<QosType> channels { get { return m_Channels; } }
        public EndPoint secureTunnelEndpoint { get { return m_EndPoint; } set { m_EndPoint = value; } }

        public bool useWebSockets { get { return m_UseWebSockets; } set { m_UseWebSockets = value; } }
        public bool useSimulator { get { return m_UseSimulator; } set { m_UseSimulator = value; } }
        public int simulatedLatency { get { return m_SimulatedLatency; } set { m_SimulatedLatency = value; } }
        public float packetLossPercentage { get { return m_PacketLossPercentage; } set { m_PacketLossPercentage = value; } }

        public string matchHost { get { return m_MatchHost; } set { m_MatchHost = value; } }
        public int matchPort { get { return m_MatchPort; } set { m_MatchPort = value; } }

        // only really valid on the server
        public int numPlayers
        {
            get
            {
                int numPlayers = 0;
                foreach (var conn in NetworkServer.connections)
                {
                    if (conn == null)
                        continue;

                    foreach (var p in conn.playerControllers)
                    {
                        if (p.IsValid)
                        {
                            numPlayers += 1;
                        }
                    }
                }
                foreach (var conn in NetworkServer.localConnections)
                {
                    if (conn == null)
                        continue;

                    foreach (var p in conn.playerControllers)
                    {
                        if (p.IsValid)
                        {
                            numPlayers += 1;
                        }
                    }
                }
                return numPlayers;
            }
        }

        // runtime data
        static public string networkSceneName = "";
        public bool isNetworkActive;
        public NetworkClient client;
        static List<Transform> s_StartPositions = new List<Transform>();
        static int s_StartPositionIndex;

        // matchmaking runtime data
        public MatchInfo matchInfo;
        public NetworkMatch matchMaker;
        public string matchName = "default";
        public uint matchSize = 4;
        public static JB_NetworkManager singleton;

        // static message objects to avoid runtime-allocations
        static AddPlayerMessage s_AddPlayerMessage = new AddPlayerMessage();
        static RemovePlayerMessage s_RemovePlayerMessage = new RemovePlayerMessage();
        static ErrorMessage s_ErrorMessage = new ErrorMessage();

        static AsyncOperation s_LoadingSceneAsync;
        static NetworkConnection s_ClientReadyConnection;

        // this is used to persist network address between scenes.
        static string s_Address;

        // Johannes blindly added these variables
        [HideInInspector]
        public GameObject hostObject;
        public List<GameObject> readyPlayers = new List<GameObject>();
        public LayoutManager lm;

        void Awake()
        {
            // do this early
            LogFilter.currentLogLevel = (int)m_LogLevel;

            if (m_DontDestroyOnLoad)
            {
                if (singleton != null)
                {
                    if (LogFilter.logWarn) { Debug.LogWarning("Multiple NetworkManagers detected in the scene. Only one NetworkManager can exist at a time. The duplicate NetworkManager will not be used."); }
                    Destroy(gameObject);
                    return;
                }
                singleton = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                singleton = this;
            }

            if (m_NetworkAddress != "")
            {
                s_Address = m_NetworkAddress;
            }
            else if (s_Address != "")
            {
                m_NetworkAddress = s_Address;
            }
        }

        void OnValidate()
        {
            if (m_SimulatedLatency < 1) m_SimulatedLatency = 1;
            if (m_SimulatedLatency > 500) m_SimulatedLatency = 500;

            if (m_PacketLossPercentage < 0) m_PacketLossPercentage = 0;
            if (m_PacketLossPercentage > 99) m_PacketLossPercentage = 99;

            if (m_MaxConnections <= 0) m_MaxConnections = 1;
            if (m_MaxConnections > 32000) m_MaxConnections = 32000;

            if (m_PlayerPrefab != null && m_PlayerPrefab.GetComponent<NetworkIdentity>() == null)
            {
                if (LogFilter.logError) { Debug.LogError("NetworkManager - playerPrefab must have a NetworkIdentity."); }
                m_PlayerPrefab = null;
            }
        }

        internal void RegisterServerMessages()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnectInternal);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnectInternal);
            NetworkServer.RegisterHandler(MsgType.Ready, OnServerReadyMessageInternal);
            NetworkServer.RegisterHandler(MsgType.AddPlayer, OnServerAddPlayerMessageInternal);
            NetworkServer.RegisterHandler(MsgType.RemovePlayer, OnServerRemovePlayerMessageInternal);
            NetworkServer.RegisterHandler(MsgType.Error, OnServerErrorInternal);
        }

        public bool StartServer(ConnectionConfig config, int maxConnections)
        {
            return StartServer(null, config, maxConnections);
        }

        public bool StartServer()
        {
            return StartServer(null);
        }

        public bool StartServer(MatchInfo info)
        {
            return StartServer(info, null, -1);
        }

        bool StartServer(MatchInfo info, ConnectionConfig config, int maxConnections)
        {
            OnStartServer();

            if (m_RunInBackground)
                Application.runInBackground = true;

            NetworkCRC.scriptCRCCheck = scriptCRCCheck;

            // passing a config overrides setting the connectionConfig property
            if (m_CustomConfig && m_ConnectionConfig != null && config == null)
            {
                m_ConnectionConfig.Channels.Clear();
                foreach (var c in m_Channels)
                {
                    m_ConnectionConfig.AddChannel(c);
                }
                NetworkServer.Configure(m_ConnectionConfig, m_MaxConnections);
            }

            RegisterServerMessages();
            NetworkServer.sendPeerInfo = m_SendPeerInfo;
            NetworkServer.useWebSockets = m_UseWebSockets;

            if (config != null)
            {
                NetworkServer.Configure(config, maxConnections);
            }

            if (info != null)
            {
                if (!NetworkServer.Listen(info, m_NetworkPort))
                {
                    if (LogFilter.logError) { Debug.LogError("StartServer listen failed."); }
                    return false;
                }
            }
            else
            {
                if (m_ServerBindToIP && !string.IsNullOrEmpty(m_ServerBindAddress))
                {
                    if (!NetworkServer.Listen(m_ServerBindAddress, m_NetworkPort))
                    {
                        if (LogFilter.logError) { Debug.LogError("StartServer listen on " + m_ServerBindAddress + " failed."); }
                        return false;
                    }
                }
                else
                {
                    if (!NetworkServer.Listen(m_NetworkPort))
                    {
                        if (LogFilter.logError) { Debug.LogError("StartServer listen failed."); }
                        return false;
                    }
                }
            }

            if (LogFilter.logDebug) { Debug.Log("NetworkManager StartServer port:" + m_NetworkPort); }
            isNetworkActive = true;

            // Only change scene if the requested online scene is not blank, and is not already loaded
            if (m_OnlineScene != "" && m_OnlineScene != Application.loadedLevelName && m_OnlineScene != m_OfflineScene)
            {
                ServerChangeScene(m_OnlineScene);
            }
            else
            {
                NetworkServer.SpawnObjects();
            }
            return true;
        }

        internal void RegisterClientMessages(NetworkClient client)
        {
            client.RegisterHandler(MsgType.Connect, OnClientConnectInternal);
            client.RegisterHandler(MsgType.Disconnect, OnClientDisconnectInternal);
            client.RegisterHandler(MsgType.NotReady, OnClientNotReadyMessageInternal);
            client.RegisterHandler(MsgType.Error, OnClientErrorInternal);
            client.RegisterHandler(MsgType.Scene, OnClientSceneInternal);

            if (m_PlayerPrefab != null)
            {
                ClientScene.RegisterPrefab(m_PlayerPrefab);
            }
            foreach (var prefab in m_SpawnPrefabs)
            {
                if (prefab != null)
                {
                    ClientScene.RegisterPrefab(prefab);
                }
            }
        }

        public void UseExternalClient(NetworkClient externalClient)
        {
            if (m_RunInBackground)
                Application.runInBackground = true;

            isNetworkActive = true;

            client = externalClient;
            RegisterClientMessages(client);
            OnStartClient(client);
            s_Address = m_NetworkAddress;
        }

        public NetworkClient StartClient(MatchInfo info, ConnectionConfig config)
        {
            matchInfo = info;
            if (m_RunInBackground)
                Application.runInBackground = true;

            isNetworkActive = true;

            client = new NetworkClient();

            if (config != null)
            {
                client.Configure(config, 1);
            }
            else
            {
                if (m_CustomConfig && m_ConnectionConfig != null)
                {
                    m_ConnectionConfig.Channels.Clear();
                    foreach (var c in m_Channels)
                    {
                        m_ConnectionConfig.AddChannel(c);
                    }
                    client.Configure(m_ConnectionConfig, m_MaxConnections);
                }
            }

            RegisterClientMessages(client);
            if (matchInfo != null)
            {
                if (LogFilter.logDebug) { Debug.Log("NetworkManager StartClient match: " + matchInfo); }
                client.Connect(matchInfo);
            }
            else if (m_EndPoint != null)
            {
                if (LogFilter.logDebug) { Debug.Log("NetworkManager StartClient using provided SecureTunnel"); }
                client.Connect(m_EndPoint);
            }
            else
            {
                if (string.IsNullOrEmpty(m_NetworkAddress))
                {
                    if (LogFilter.logError) { Debug.LogError("Must set the Network Address field in the manager"); }
                    return null;
                }
                if (LogFilter.logDebug) { Debug.Log("NetworkManager StartClient address:" + m_NetworkAddress + " port:" + m_NetworkPort); }

                if (m_UseSimulator)
                {
                    client.ConnectWithSimulator(m_NetworkAddress, m_NetworkPort, m_SimulatedLatency, m_PacketLossPercentage);
                }
                else
                {
                    client.Connect(m_NetworkAddress, m_NetworkPort);
                }
            }
            OnStartClient(client);
            s_Address = m_NetworkAddress;
            return client;
        }

        public NetworkClient StartClient(MatchInfo matchInfo)
        {
            return StartClient(matchInfo, null);
        }

        public NetworkClient StartClient()
        {
            return StartClient(null, null);
        }

        public virtual NetworkClient StartHost(ConnectionConfig config, int maxConnections)
        {
            OnStartHost();
            if (StartServer(config, maxConnections))
            {
                var client = ConnectLocalClient();
                OnServerConnect(client.connection);
                OnStartClient(client);
                return client;
            }
            return null;
        }

        public virtual NetworkClient StartHost(MatchInfo info)
        {
            OnStartHost();
            matchInfo = info;
            if (StartServer(info))
            {
                var client = ConnectLocalClient();
                OnServerConnect(client.connection);
                OnStartClient(client);
                return client;
            }
            return null;
        }

        public virtual NetworkClient StartHost()
        {
            OnStartHost();
            if (StartServer())
            {
                var localClient = ConnectLocalClient();
                OnServerConnect(localClient.connection);
                OnStartClient(localClient);
                return localClient;
            }
            return null;
        }

        NetworkClient ConnectLocalClient()
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager StartHost port:" + m_NetworkPort); }
            m_NetworkAddress = "localhost";
            client = ClientScene.ConnectLocalServer();
            RegisterClientMessages(client);
            return client;
        }

        public void StopHost()
        {
            OnStopHost();

            StopServer();
            StopClient();
        }

        public void StopServer()
        {
            if (!NetworkServer.active)
                return;

            OnStopServer();

            if (LogFilter.logDebug) { Debug.Log("NetworkManager StopServer"); }
            isNetworkActive = false;
            NetworkServer.Shutdown();
            StopMatchMaker();

            if (m_OfflineScene != "")
            {
                ServerChangeScene(m_OfflineScene);
            }
        }

        public void StopClient()
        {
            OnStopClient();

            if (LogFilter.logDebug) { Debug.Log("NetworkManager StopClient"); }
            isNetworkActive = false;
            if (client != null)
            {
                // only shutdown this client, not ALL clients.
                client.Disconnect();
                client.Shutdown();
                client = null;
            }
            StopMatchMaker();

            ClientScene.DestroyAllClientObjects();
            if (m_OfflineScene != "")
            {
                ClientChangeScene(m_OfflineScene, false);
            }
        }

        public virtual void ServerChangeScene(string newSceneName)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                if (LogFilter.logError) { Debug.LogError("ServerChangeScene empty scene name"); }
                return;
            }

            if (LogFilter.logDebug) { Debug.Log("ServerChangeScene " + newSceneName); }
            NetworkServer.SetAllClientsNotReady();
            networkSceneName = newSceneName;

            s_LoadingSceneAsync = Application.LoadLevelAsync(newSceneName);

            StringMessage msg = new StringMessage(networkSceneName);
            NetworkServer.SendToAll(MsgType.Scene, msg);

            s_StartPositionIndex = 0;
            s_StartPositions.Clear();
        }

        internal void ClientChangeScene(string newSceneName, bool forceReload)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                if (LogFilter.logError) { Debug.LogError("ClientChangeScene empty scene name"); }
                return;
            }

            if (LogFilter.logDebug) { Debug.Log("ClientChangeScene newSceneName:" + newSceneName + " networkSceneName:" + networkSceneName); }
            if (newSceneName == networkSceneName && !forceReload)
            {
                // this is automatic now
                //ClientScene.PrepareSpawnObjects();
                return;
            }

            s_LoadingSceneAsync = Application.LoadLevelAsync(newSceneName);
            networkSceneName = newSceneName;
        }

        void FinishLoadScene()
        {
            // NOTE: this cannot use NetworkClient.allClients[0] - that client may be for a completely different purpose.

            if (client != null)
            {
                if (s_ClientReadyConnection != null)
                {
                    OnClientConnect(s_ClientReadyConnection);
                    s_ClientReadyConnection = null;
                }
            }

            if (NetworkServer.active)
            {
                NetworkServer.SpawnObjects();
                OnServerSceneChanged(networkSceneName);
            }

            if (IsClientConnected() && client != null)
            {
                RegisterClientMessages(client);
                OnClientSceneChanged(client.connection);
            }
        }

        internal static void UpdateScene()
        {
            if (singleton == null)
                return;

            if (s_LoadingSceneAsync == null)
                return;

            if (!s_LoadingSceneAsync.isDone)
                return;

            if (LogFilter.logDebug) { Debug.Log("ClientChangeScene done readyCon:" + s_ClientReadyConnection); }
            singleton.FinishLoadScene();
            s_LoadingSceneAsync.allowSceneActivation = true;
            s_LoadingSceneAsync = null;
        }

        static public void RegisterStartPosition(Transform start)
        {
            if (LogFilter.logDebug) { Debug.Log("RegisterStartPosition:" + start); }
            s_StartPositions.Add(start);
        }

        static public void UnRegisterStartPosition(Transform start)
        {
            if (LogFilter.logDebug) { Debug.Log("UnRegisterStartPosition:" + start); }
            s_StartPositions.Remove(start);
        }

        public bool IsClientConnected()
        {
            return client != null && client.isConnected;
        }

        // this is the only way to clear the singleton, so another instance can be created.
        static public void Shutdown()
        {
            if (singleton == null)
                return;

            s_StartPositions.Clear();
            s_StartPositionIndex = 0;
            s_ClientReadyConnection = null;

            singleton.StopHost();
            singleton = null;
        }

        // ----------------------------- Server Internal Message Handlers  --------------------------------

        internal void OnServerConnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnServerConnectInternal"); }

            netMsg.conn.SetMaxDelay(m_MaxDelay);

            if (networkSceneName != "" && networkSceneName != m_OfflineScene)
            {
                StringMessage msg = new StringMessage(networkSceneName);
                netMsg.conn.Send(MsgType.Scene, msg);
            }
            OnServerConnect(netMsg.conn);
        }

        internal void OnServerDisconnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnServerDisconnectInternal"); }

            OnServerDisconnect(netMsg.conn);
        }

        internal void OnServerReadyMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnServerReadyMessageInternal"); }

            OnServerReady(netMsg.conn);
        }

        internal void OnServerAddPlayerMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnServerAddPlayerMessageInternal"); }

            netMsg.ReadMessage(s_AddPlayerMessage);

            if (s_AddPlayerMessage.msgSize != 0)
            {
                var reader = new NetworkReader(s_AddPlayerMessage.msgData);
                OnServerAddPlayer(netMsg.conn, s_AddPlayerMessage.playerControllerId, reader);
            }
            else
            {
                OnServerAddPlayer(netMsg.conn, s_AddPlayerMessage.playerControllerId);
            }
        }

        internal void OnServerRemovePlayerMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnServerRemovePlayerMessageInternal"); }

            netMsg.ReadMessage(s_RemovePlayerMessage);

            //PlayerController player;
            //netMsg.conn.GetPlayerController(s_RemovePlayerMessage.playerControllerId, out player);
            //OnServerRemovePlayer(netMsg.conn, player);
            //netMsg.conn.RemovePlayerController(s_RemovePlayerMessage.playerControllerId);
        }

        internal void OnServerErrorInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnServerErrorInternal"); }

            netMsg.ReadMessage(s_ErrorMessage);
            OnServerError(netMsg.conn, s_ErrorMessage.errorCode);
        }

        // ----------------------------- Client Internal Message Handlers  --------------------------------

        internal void OnClientConnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnClientConnectInternal"); }

            netMsg.conn.SetMaxDelay(m_MaxDelay);

            if (string.IsNullOrEmpty(m_OnlineScene) || (m_OnlineScene == m_OfflineScene))
            {
                OnClientConnect(netMsg.conn);
            }
            else
            {
                // will wait for scene id to come from the server.
                s_ClientReadyConnection = netMsg.conn;
            }
        }

        internal void OnClientDisconnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnClientDisconnectInternal"); }

            if (m_OfflineScene != "")
            {
                ClientChangeScene(m_OfflineScene, false);
            }
            OnClientDisconnect(netMsg.conn);
        }

        internal void OnClientNotReadyMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnClientNotReadyMessageInternal"); }

            //ClientScene.SetNotReady();
            OnClientNotReady(netMsg.conn);

            // NOTE: s_ClientReadyConnection is not set here! don't want OnClientConnect to be invoked again after scene changes.
        }

        internal void OnClientErrorInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnClientErrorInternal"); }

            netMsg.ReadMessage(s_ErrorMessage);
            OnClientError(netMsg.conn, s_ErrorMessage.errorCode);
        }

        internal void OnClientSceneInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager:OnClientSceneInternal"); }

            string newSceneName = netMsg.reader.ReadString();

            if (IsClientConnected() && !NetworkServer.active)
            {
                ClientChangeScene(newSceneName, true);
            }
        }

        // ----------------------------- Server System Callbacks --------------------------------

        public virtual void OnServerConnect(NetworkConnection conn)
        {
        }

        public virtual void OnServerDisconnect(NetworkConnection conn)
        {
            NetworkServer.DestroyPlayersForConnection(conn);
        }

        public virtual void OnServerReady(NetworkConnection conn)
        {
            if (conn.playerControllers.Count == 0)
            {
                // this is now allowed (was not for a while)
                if (LogFilter.logDebug) { Debug.Log("Ready with no player object"); }
            }
            NetworkServer.SetClientReady(conn);
        }

        public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            OnServerAddPlayerInternal(conn, playerControllerId);
        }

        public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            OnServerAddPlayerInternal(conn, playerControllerId);
        }

        void OnServerAddPlayerInternal(NetworkConnection conn, short playerControllerId)
        {
            if (m_PlayerPrefab == null)
            {
                if (LogFilter.logError) { Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object."); }
                return;
            }

            if (m_PlayerPrefab.GetComponent<NetworkIdentity>() == null)
            {
                if (LogFilter.logError) { Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab."); }
                return;
            }

            if (playerControllerId < conn.playerControllers.Count && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
            {
                if (LogFilter.logError) { Debug.LogError("There is already a player at that playerControllerId for this connections."); }
                return;
            }

            GameObject player;
            Transform startPos = GetStartPosition();
            if (startPos != null)
            {
                player = (GameObject)Instantiate(m_PlayerPrefab, startPos.position, startPos.rotation);
            }
            else
            {
                player = (GameObject)Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);
            }

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

            // My additions
            if (hostObject)
            {
                hostObject.GetComponent<PCControl>().UpdateLaunchButton(readyPlayers.Count, (numPlayers - 2));
            }
            else
            {
                hostObject = player;
            }

            PCControl _pc = player.GetComponent<PCControl>();
            _pc.RpcSpawnPlayerSelectScreen();
            _pc.SetHostLaunchButton();
            _pc.nm = this;
        }

        public Transform GetStartPosition()
        {
            // first remove any dead transforms
            if (s_StartPositions.Count > 0)
            {
                for (int i = s_StartPositions.Count - 1; i >= 0; i--)
                {
                    if (s_StartPositions[i] == null)
                        s_StartPositions.RemoveAt(i);
                }
            }

            if (m_PlayerSpawnMethod == PlayerSpawnMethod.Random && s_StartPositions.Count > 0)
            {
                // try to spawn at a random start location
                int index = Random.Range(0, s_StartPositions.Count);
                return s_StartPositions[index];
            }
            if (m_PlayerSpawnMethod == PlayerSpawnMethod.RoundRobin && s_StartPositions.Count > 0)
            {
                if (s_StartPositionIndex >= s_StartPositions.Count)
                {
                    s_StartPositionIndex = 0;
                }

                Transform startPos = s_StartPositions[s_StartPositionIndex];
                s_StartPositionIndex += 1;
                return startPos;
            }
            return null;
        }

        public virtual void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            if (player.gameObject != null)
            {
                NetworkServer.Destroy(player.gameObject);
            }
        }

        public virtual void OnServerError(NetworkConnection conn, int errorCode)
        {
        }

        public virtual void OnServerSceneChanged(string sceneName)
        {
        }

        // ----------------------------- Client System Callbacks --------------------------------

        public virtual void OnClientConnect(NetworkConnection conn)
        {
            if (string.IsNullOrEmpty(m_OnlineScene) || (m_OnlineScene == m_OfflineScene))
            {
                ClientScene.Ready(conn);
                if (m_AutoCreatePlayer)
                {
                    ClientScene.AddPlayer(0);
                }
            }
        }

        public virtual void OnClientDisconnect(NetworkConnection conn)
        {
            StopClient();
        }

        public virtual void OnClientError(NetworkConnection conn, int errorCode)
        {
        }

        public virtual void OnClientNotReady(NetworkConnection conn)
        {
        }

        public virtual void OnClientSceneChanged(NetworkConnection conn)
        {
            // always become ready.
            ClientScene.Ready(conn);

            if (!m_AutoCreatePlayer)
            {
                return;
            }

            bool addPlayer = (ClientScene.localPlayers.Count == 0);
            bool foundPlayer = false;
            foreach (var playerController in ClientScene.localPlayers)
            {
                if (playerController.gameObject != null)
                {
                    foundPlayer = true;
                    break;
                }
            }
            if (!foundPlayer)
            {
                // there are players, but their game objects have all been deleted
                addPlayer = true;
            }
            if (addPlayer)
            {
                ClientScene.AddPlayer(0);
            }
        }

        // ----------------------------- Matchmaker --------------------------------

        public void StartMatchMaker()
        {
            if (LogFilter.logDebug) { Debug.Log("NetworkManager StartMatchMaker"); }
            SetMatchHost(m_MatchHost, m_MatchPort, true);
        }

        public void StopMatchMaker()
        {
            if (matchMaker != null)
            {
                Destroy(matchMaker);
                matchMaker = null;
            }
            matchInfo = null;
            //matches = null;
        }

        public void SetMatchHost(string newHost, int port, bool https)
        {
            if (matchMaker == null)
            {
                matchMaker = gameObject.AddComponent<NetworkMatch>();
            }
            if (newHost == "localhost" || newHost == "127.0.0.1")
            {
                newHost = Environment.MachineName;
            }
            string prefix = "http://";
            if (https)
            {
                prefix = "https://";
            }

            if (LogFilter.logDebug) { Debug.Log("SetMatchHost:" + newHost); }
            m_MatchHost = newHost;
            m_MatchPort = port;
            matchMaker.baseUri = new Uri(prefix + m_MatchHost + ":" + m_MatchPort);
        }

        //------------------------------ Start & Stop callbacks -----------------------------------

        // Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
        // their functionality, users would need override all the versions. Instead these callbacks are invoked
        // from all versions, so users only need to implement this one case.

        public virtual void OnStartHost()
        {
        }

        public virtual void OnStartServer()
        {
        }

        public virtual void OnStartClient(NetworkClient client)
        {
            
        }

        public virtual void OnStopServer()
        {
        }

        public virtual void OnStopClient()
        {
        }

        public virtual void OnStopHost()
        {
        }

        public void SetPlayerReady(GameObject _player)
        {
            readyPlayers.Add(_player);
            hostObject.GetComponent<PCControl>().UpdateLaunchButton(readyPlayers.Count, numPlayers - 2);
        }

        public void HostReadyToPlay()
        {
            readyPlayers.Add(hostObject);
            foreach (GameObject player in readyPlayers)
            {
                player.GetComponent<PCControl>().RpcBeginGame();
            }
            lm.BeginTheSpawning(readyPlayers);
        }

        public void BuildRoomOnClient(int roomIndex, Vector3 pos)
        {
            foreach (GameObject player in readyPlayers)
            {
                player.GetComponent<PCControl>().RpcBuildRoom(roomIndex, pos);
            }
        }
    }
}
#endif //ENABLE_UNET