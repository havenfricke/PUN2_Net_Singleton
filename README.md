### Net Singleton API Reference (Photon PUN)

Pairs extremely well with my [Unity 6 input module](https://github.com/havenfricke/Unity6_PlayerInputModule).

A clean, **single-responsibility network facade** for Unity + Photon PUN.

`Net.cs` singleton describes the public API surface of Net, including required arguments, intent, and when to use each method. Gameplay code should interact only with Net.Instance and (locally) its own PhotonView. `Net.cs` does not need to be included in the Unity scene hierarchy and can be called from anywhere in the application (see "Design Contract" below).

**Scene Network GameObject** folder contains `NetAutoJoin.cs`, the main script that handles joining the server. It also contains two optional helpers `NetIdentityDebug.cs` and `NetPingTest.cs`. These go on one `GameObject` in the scene hierarchy. 

**Scene Spawn GameObject** folder contains `PlayerSpawner.cs`. A script that asynchronously handles unavailable spawn points. Place it on a `GameObject` in the scene hierarchy separate from `NetAutoJoin.cs` and other debugging scripts mentioned prior. A subfolder contains `AvailableSpawnHandler.cs` that should be placed on each child `GameObject` (spawn points) where `PlayerSpawner.cs` is located. Once those are added, `PlayerSpawner` has available indecies to add the spawn point child objects. 

Best practice is to keep `PlayerSpawner.cs` at `Vector3(0, 0, 0)` in scene and individual spawn points at desired locations.

---

### Design Contract
- Call Net.Instance
- Own a PhotonView
- Never reference PhotonNetwork

### Net.cs
- Owns Photon callbacks
- Connects PhotonNetwork
- Routes intent (connect, sync, events)

---

### Connection & Session Flow

---

```csharp
bool ConnectUsingSettings()
```

Connects using values from PhotonServerSettings

Use at application startup.

---

```csharp
bool ConnectUsingSettings(AppSettings appSettings, bool startInOfflineMode)
```

Connects using explicit settings (testing / overrides).

---

```csharp
bool ConnectToMaster(string masterServerAddress, int port, string appId)
```

Direct connection to a specific master server.

---

```csharp
bool ConnectToBestCloudServer()
```

Automatically selects the best region.

---

```csharp
bool ConnectToRegion(string region)
```

Forces a specific region (e.g., us, eu).

---

```csharp
void DisconnectFromServer()
```

Gracefully disconnects from Photon.

---

```csharp
bool Reconnect()
```

Reconnects using cached connection parameters.

---

```csharp
bool ReconnectAndRejoin()
```

Reconnects and attempts to rejoin the previous room.

---

### Lobby & Room Management

---

```csharp
bool JoinLobby()
```

Joins the default lobby.

---

```csharp
bool JoinLobby(TypedLobby typedLobby)
```

Joins a specific lobby configuration.

---

```csharp
bool LeaveLobby()
```

Leaves the current lobby.

---

```csharp
bool CreateRoom(
    string roomName,
    RoomOptions roomOptions,
    TypedLobby typedLobby,
    string[] expectedUsers
)
```

Creates a room with explicit configuration.

---

```csharp
bool JoinRoom(string roomName, string[] expectedUsers)
```

Joins an existing room by name.

---

```csharp
bool JoinOrCreateRoom(
    string roomName,
    RoomOptions roomOptions,
    TypedLobby typedLobby,
    string[] expectedUsers
)
```

Primary matchmaking entry point.

---

```csharp
bool JoinRandomRoom()
```

Joins any available matching room.

---

```csharp
bool LeaveRoom(bool becomeInactive)
```

Leaves the room; optionally preserves state for rejoin.

---

```csharp
bool SetMasterClient(Player masterClientPlayer)
```

Transfers master-client authority.

---

```csharp
bool CloseConnection(Player kickPlayer)
```

Forcibly removes a player from the room.

---

### Networked Object Lifecycle

---

```csharp
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

```csharp
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

```csharp
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

```csharp
void Destroy(GameObject targetGameObject)
```

Destroys a networked object by reference.

---

```csharp
void Destroy(PhotonView targetView)
```

Destroys a networked object by its view.

---

```csharp
void DestroyAll()
```

Destroys all networked objects in the room.

---

```csharp
void DestroyPlayerObjects(Player targetPlayer)
```

Destroys all objects owned by a specific player.

---

```csharp
void DestroyPlayerObjects(int targetPlayerId)
```

Destroys objects using the player's actor number.

---

### RPC Facade (Deterministic State Sync)

---

```csharp
void Rpc(
    PhotonView view,
    string methodName,
    RpcTarget target,
    object[] parameters
)
```

Invokes a PunRPC method on one or more clients.
Use for authoritative gameplay state.

---

```csharp
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

```csharp
void RpcToPlayer(
    PhotonView view,
    string methodName,
    Player targetPlayer,
    object[] parameters
)
```

Targets a single specific player.

---

```csharp
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

---

```csharp
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

```csharp
void RegisterEventHandler(
    byte eventCode,
    Action<EventData> handler
)
```

Subscribes to a specific event code.

---

```csharp
void UnregisterEventHandler(
    byte eventCode,
    Action<EventData> handler
)
```

Unsubscribes from an event code.

---

### Performance & Networking Control

---

```csharp
void SetSendRates(int sendRate, int serializationRate)
```

Controls network tick rate and serialization frequency.

---

```csharp
void SendAllOutgoingCommands()
```

Immediately flushes outgoing network traffic.

---

### Read-Only State Accessors

---

```csharp
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

