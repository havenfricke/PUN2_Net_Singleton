/*
    Net.cs
    A single-responsibility singleton "network facade" for Photon PUN.

    Goals:
    - Call Net.Instance from anywhere, like a single-player service.
    - Centralize PhotonNetwork / PhotonView operations behind explicit methods.
    - Provide strongly-defined lifecycle events for connection and room flow.
    - Provide a simple event-code router for RaiseEvent/OnEvent.

    Notes:
    - PhotonNetwork is static; this wrapper exists to keep the rest of your codebase clean.
    - PhotonView.RPC is the core for RPC calls; RaiseEvent is the core for lightweight custom events.

    References:
    - PhotonNetwork: ConnectUsingSettings, Disconnect, JoinOrCreateRoom, Instantiate, Destroy, RaiseEvent, AddCallbackTarget, RemoveCallbackTarget, etc.
    - PhotonView: RPC, RpcSecure
*/

using System;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public sealed class Net : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private static Net instance;
    private static readonly object instanceLock = new object();

    private readonly Dictionary<byte, Action<EventData>> eventHandlers;

    public static Net Instance
    {
        get
        {
            EnsureInstanceExists();
            return Net.instance;
        }
    }

    public event Action ConnectedToMaster;
    public event Action Disconnected;
    public event Action JoinedLobby;
    public event Action LeftLobby;
    public event Action CreatedRoom;
    public event Action JoinedRoom;
    public event Action LeftRoom;
    public event Action<Player> PlayerEnteredRoom;
    public event Action<Player> PlayerLeftRoom;

    private void Awake()
    {
        if (Net.instance != null && Net.instance != this)
        {
            UnityEngine.Object.Destroy(this.gameObject);
            return;
        }

        Net.instance = this;
        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);

        this.eventHandlers = new Dictionary<byte, Action<EventData>>();

        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        if (Net.instance == this)
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            Net.instance = null;
        }
    }

    private static void EnsureInstanceExists()
    {
        if (Net.instance != null)
        {
            return;
        }

        lock (Net.instanceLock)
        {
            if (Net.instance != null)
            {
                return;
            }

            Net existing = UnityEngine.Object.FindFirstObjectByType<Net>();
            if (existing != null)
            {
                Net.instance = existing;
                return;
            }

            GameObject host = new GameObject("Net");
            Net created = host.AddComponent<Net>();
            Net.instance = created;
        }
    }

    public bool IsConnected
    {
        get { return PhotonNetwork.IsConnected; }
    }

    public bool IsConnectedAndReady
    {
        get { return PhotonNetwork.IsConnectedAndReady; }
    }

    public bool InRoom
    {
        get { return PhotonNetwork.InRoom; }
    }

    public bool InLobby
    {
        get { return PhotonNetwork.InLobby; }
    }

    public ClientState NetworkClientState
    {
        get { return PhotonNetwork.NetworkClientState; }
    }

    public Player LocalPlayer
    {
        get { return PhotonNetwork.LocalPlayer; }
    }

    public Room CurrentRoom
    {
        get { return PhotonNetwork.CurrentRoom; }
    }

    public int ServerPing
    {
        get { return PhotonNetwork.GetPing(); }
    }

    // --------------------------------------

    public bool ConnectUsingSettings()
    {
        bool result = PhotonNetwork.ConnectUsingSettings();
        return result;
    }

    public bool ConnectUsingSettings(AppSettings appSettings, bool startInOfflineMode)
    {
        bool result = PhotonNetwork.ConnectUsingSettings(appSettings, startInOfflineMode);
        return result;
    }

    public bool ConnectToMaster(string masterServerAddress, int port, string appId)
    {
        bool result = PhotonNetwork.ConnectToMaster(masterServerAddress, port, appId);
        return result;
    }

    public bool ConnectToBestCloudServer()
    {
        bool result = PhotonNetwork.ConnectToBestCloudServer();
        return result;
    }

    public bool ConnectToRegion(string region)
    {
        bool result = PhotonNetwork.ConnectToRegion(region);
        return result;
    }

    public void DisconnectFromServer()
    {
        PhotonNetwork.Disconnect();
    }

    public bool Reconnect()
    {
        bool result = PhotonNetwork.Reconnect();
        return result;
    }

    public bool ReconnectAndRejoin()
    {
        bool result = PhotonNetwork.ReconnectAndRejoin();
        return result;
    }

    public bool JoinLobby()
    {
        bool result = PhotonNetwork.JoinLobby();
        return result;
    }

    public bool JoinLobby(TypedLobby typedLobby)
    {
        bool result = PhotonNetwork.JoinLobby(typedLobby);
        return result;
    }

    public bool LeaveLobby()
    {
        bool result = PhotonNetwork.LeaveLobby();
        return result;
    }

    public bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
    {
        bool result = PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby, expectedUsers);
        return result;
    }

    public bool JoinRoom(string roomName, string[] expectedUsers)
    {
        bool result = PhotonNetwork.JoinRoom(roomName, expectedUsers);
        return result;
    }

    public bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
    {
        bool result = PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby, expectedUsers);
        return result;
    }

    public bool JoinRandomRoom()
    {
        bool result = PhotonNetwork.JoinRandomRoom();
        return result;
    }

    public bool LeaveRoom(bool becomeInactive)
    {
        bool result = PhotonNetwork.LeaveRoom(becomeInactive);
        return result;
    }

    public bool SetMasterClient(Player masterClientPlayer)
    {
        bool result = PhotonNetwork.SetMasterClient(masterClientPlayer);
        return result;
    }

    public bool CloseConnection(Player kickPlayer)
    {
        bool result = PhotonNetwork.CloseConnection(kickPlayer);
        return result;
    }

    public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data)
    {
        GameObject spawned = PhotonNetwork.Instantiate(prefabName, position, rotation, group, data);
        return spawned;
    }

    public GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data)
    {
        GameObject spawned = PhotonNetwork.InstantiateSceneObject(prefabName, position, rotation, group, data);
        return spawned;
    }

    public GameObject InstantiateRoomObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data)
    {
        GameObject spawned = PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation, group, data);
        return spawned;
    }

    public void Destroy(GameObject targetGameObject)
    {
        PhotonNetwork.Destroy(targetGameObject);
    }

    public void Destroy(PhotonView targetView)
    {
        PhotonNetwork.Destroy(targetView);
    }

    public void DestroyAll()
    {
        PhotonNetwork.DestroyAll();
    }

    public void DestroyPlayerObjects(Player targetPlayer)
    {
        PhotonNetwork.DestroyPlayerObjects(targetPlayer);
    }

    public void DestroyPlayerObjects(int targetPlayerId)
    {
        PhotonNetwork.DestroyPlayerObjects(targetPlayerId);
    }

    public void Rpc(PhotonView view, string methodName, RpcTarget target, object[] parameters)
    {
        if (view == null)
        {
            throw new ArgumentNullException("view");
        }

        if (string.IsNullOrEmpty(methodName))
        {
            throw new ArgumentException("methodName must be non-empty.", "methodName");
        }

        if (parameters == null)
        {
            parameters = new object[0];
        }

        view.RPC(methodName, target, parameters);
    }

    public void RpcSecure(PhotonView view, string methodName, RpcTarget target, bool encrypt, object[] parameters)
    {
        if (view == null)
        {
            throw new ArgumentNullException("view");
        }

        if (string.IsNullOrEmpty(methodName))
        {
            throw new ArgumentException("methodName must be non-empty.", "methodName");
        }

        if (parameters == null)
        {
            parameters = new object[0];
        }

        view.RpcSecure(methodName, target, encrypt, parameters);
    }

    public void RpcToPlayer(PhotonView view, string methodName, Player targetPlayer, object[] parameters)
    {
        if (view == null)
        {
            throw new ArgumentNullException("view");
        }

        if (string.IsNullOrEmpty(methodName))
        {
            throw new ArgumentException("methodName must be non-empty.", "methodName");
        }

        if (targetPlayer == null)
        {
            throw new ArgumentNullException("targetPlayer");
        }

        if (parameters == null)
        {
            parameters = new object[0];
        }

        view.RPC(methodName, targetPlayer, parameters);
    }

    public void RpcSecureToPlayer(PhotonView view, string methodName, Player targetPlayer, bool encrypt, object[] parameters)
    {
        if (view == null)
        {
            throw new ArgumentNullException("view");
        }

        if (string.IsNullOrEmpty(methodName))
        {
            throw new ArgumentException("methodName must be non-empty.", "methodName");
        }

        if (targetPlayer == null)
        {
            throw new ArgumentNullException("targetPlayer");
        }

        if (parameters == null)
        {
            parameters = new object[0];
        }

        view.RpcSecure(methodName, targetPlayer, encrypt, parameters);
    }

    public bool RaiseEvent(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
    {
        bool result = PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
        return result;
    }

    public void RegisterEventHandler(byte eventCode, Action<EventData> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException("handler");
        }

        Action<EventData> existing;
        if (this.eventHandlers.TryGetValue(eventCode, out existing))
        {
            existing = existing + handler;
            this.eventHandlers[eventCode] = existing;
            return;
        }

        this.eventHandlers.Add(eventCode, handler);
    }

    public void UnregisterEventHandler(byte eventCode, Action<EventData> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException("handler");
        }

        Action<EventData> existing;
        if (this.eventHandlers.TryGetValue(eventCode, out existing))
        {
            existing = existing - handler;

            if (existing == null)
            {
                this.eventHandlers.Remove(eventCode);
            }
            else
            {
                this.eventHandlers[eventCode] = existing;
            }
        }
    }

    public void SetSendRates(int sendRate, int serializationRate)
    {
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = serializationRate;
    }

    public void SendAllOutgoingCommands()
    {
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void OnConnectedToMaster()
    {
        Action handler = this.ConnectedToMaster;
        if (handler != null)
        {
            handler();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Action handler = this.Disconnected;
        if (handler != null)
        {
            handler();
        }
    }

    public override void OnJoinedLobby()
    {
        Action handler = this.JoinedLobby;
        if (handler != null)
        {
            handler();
        }
    }

    public override void OnLeftLobby()
    {
        Action handler = this.LeftLobby;
        if (handler != null)
        {
            handler();
        }
    }

    public override void OnCreatedRoom()
    {
        Action handler = this.CreatedRoom;
        if (handler != null)
        {
            handler();
        }
    }

    public override void OnJoinedRoom()
    {
        Action handler = this.JoinedRoom;
        if (handler != null)
        {
            handler();
        }
    }

    public override void OnLeftRoom()
    {
        Action handler = this.LeftRoom;
        if (handler != null)
        {
            handler();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Action<Player> handler = this.PlayerEnteredRoom;
        if (handler != null)
        {
            handler(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Action<Player> handler = this.PlayerLeftRoom;
        if (handler != null)
        {
            handler(otherPlayer);
        }
    }

    // -----------------------------
    // IOnEventCallback (RaiseEvent receive)
    // -----------------------------

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent == null)
        {
            return;
        }

        byte code = photonEvent.Code;

        Action<EventData> handler;
        if (this.eventHandlers.TryGetValue(code, out handler))
        {
            if (handler != null)
            {
                handler(photonEvent);
            }
        }
    }
}
