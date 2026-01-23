### Net Singleton API Reference (Photon PUN)

Pairs extremely well with my [Unity 6 input module](https://github.com/havenfricke/Unity6_PlayerInputModule).

A clean, **single-responsibility network facade** for Unity + Photon PUN.

This document describes the **public API surface of `Net`, including required arguments, intent, and when to use each method. Gameplay code should interact only with `Net.Instance` and (locally) its own `PhotonView`.

---

### Design Contract
Gameplay scripts:
- Call `Net.Instance`
- Own a `PhotonView`
- Never reference `PhotonNetwork`

Net:
- Owns Photon callbacks
- Connects `PhotonNetwork`
- Routes intent (connect, sync, events)

---

### Connection & Session Flow

```
bool ConnectUsingSettings()
```

Connects using values from `PhotonServerSettings`

Use at application startup.

---

```
bool ConnectUsingSettings(AppSettings appSettings, bool startInOfflineMode)
```

Connects using explicit settings (testing / overrides).

---

```csharp
bool ConnectToMaster(string masterServerAddress, int port, string appId)
```

Direct connection to a specific master server.

---

```
bool ConnectToBestCloudServer()
```

Automatically selects the best region.

---

```
bool ConnectToRegion(string region)
```

Forces a specific region (e.g., us, eu).

---

```
void DisconnectFromServer()
```

Gracefully disconnects from Photon.

---

```
bool Reconnect()
```

Reconnects using cached connection parameters.

---

```
bool ReconnectAndRejoin()
```

Reconnects and attempts to rejoin the previous room.

---

### Lobby & Room Management

```
bool JoinLobby()
```

Joins the default lobby.

---

```
bool JoinLobby(TypedLobby typedLobby)
```

Joins a specific lobby configuration.

---

```
bool LeaveLobby()
```

Leaves the current lobby.

---

```
bool CreateRoom(
    string roomName,
    RoomOptions roomOptions,
    TypedLobby typedLobby,
    string[] expectedUsers
)
```

Creates a room with explicit configuration.

---

```
bool JoinRoom(string roomName, string[] expectedUsers)
```

Joins an existing room by name.

---

```
bool JoinOrCreateRoom(
    string roomName,
    RoomOptions roomOptions,
    TypedLobby typedLobby,
    string[] expectedUsers
)
```

Primary matchmaking entry point.

---

```
bool JoinRandomRoom()
```

Joins any available matching room.

---

```
bool LeaveRoom(bool becomeInactive)
```

Leaves the room; optionally preserves state for rejoin.

---

```
bool SetMasterClient(Player masterClientPlayer)
```

Transfers master-client authority.

---

```csharp
bool CloseConnection(Player kickPlayer)
```

Forcibly removes a player from the room.

---

## Networked Object Lifecycle

```
GameObject Instantiate(
    string prefabName,
    Vector3 position,
    Quaternion rotation,
    byte group,
    object[] data
)
```

Spawns a player-owned networked object.

---

```
GameObject InstantiateSceneObject(
    string prefabName,
    Vector3 position,
    Quaternion rotation,
    byte group,
    object[] data
)
```

Spawns a scene-owned persistent object.

---

```
GameObject InstantiateRoomObject(
    string prefabName,
    Vector3 position,
    Quaternion rotation,
    byte group,
    object[] data
)
```

Spawns an object owned by the room itself.

---

```
void Destroy(GameObject targetGameObject)
```

Destroys a networked object by reference.

---

```
void Destroy(PhotonView targetView)
```

Destroys a networked object by its view.

---

```
void DestroyAll()
```

Destroys all networked objects in the room.

---

```
void DestroyPlayerObjects(Player targetPlayer)
```

Destroys all objects owned by a specific player.

---

```
void DestroyPlayerObjects(int targetPlayerId)
```

Destroys objects using the player's actor number.

---

## RPC Facade (Deterministic State Sync)

```
void Rpc(
    PhotonView view,
    string methodName,
    RpcTarget target,
    object[] parameters
)
```

Invokes a PunRPC method on one or more clients.
Use for **authoritative gameplay state**.

---

```
void RpcSecure(
    PhotonView view,
    string methodName,
    RpcTarget target,
    bool encrypt,
    object[] parameters
)
```

Encrypted version of Rpc.

---

```
void RpcToPlayer(
    PhotonView view,
    string methodName,
    Player targetPlayer,
    object[] parameters
)
```

Targets a single specific player.

---

```
void RpcSecureToPlayer(
    PhotonView view,
    string methodName,
    Player targetPlayer,
    bool encrypt,
    object[] parameters
)
```

Private, encrypted RPC to one player.

---

### RaiseEvent (Transient Signals)

```
bool RaiseEvent(
    byte eventCode,
    object eventContent,
    RaiseEventOptions raiseEventOptions,
    SendOptions sendOptions
)
```

Broadcasts a lightweight room event.
Use for non-authoritative signals (FX, UI, audio).

---

```
void RegisterEventHandler(
    byte eventCode,
    Action<EventData> handler
)
```

Subscribes to a specific event code.

---

```
void UnregisterEventHandler(
    byte eventCode,
    Action<EventData> handler
)
```

Unsubscribes from an event code.

---

### Performance & Networking Control

```
void SetSendRates(int sendRate, int serializationRate)
```

Controls network tick rate and serialization frequency.

---

```
void SendAllOutgoingCommands()
```

Immediately flushes outgoing network traffic.

---

### Read-Only State Accessors

```
bool IsConnected { get; }
bool IsConnectedAndReady { get; }
bool InRoom { get; }
bool InLobby { get; }
ClientState NetworkClientState { get; }
Player LocalPlayer { get; }
Room CurrentRoom { get; }
int ServerPing { get; }
```

Use these for queries only, never for control flow.

---

