using UnityEngine;
using Photon.Realtime;

// IMPORTANT: This script is intended to go on a GameObject in the Unity scene where the player joins the server

[RequireComponent(typeof(PhotonView))]
public sealed class NetAutoJoin : MonoBehaviour
{
    [SerializeField]
    private string roomName = "dev-room";

    private void OnEnable()
    {
        Net.Instance.ConnectedToMaster += this.HandleConnectedToMaster;
        Net.Instance.JoinedRoom += this.HandleJoinedRoom;
        Net.Instance.Disconnected += this.HandleDisconnected;
    }

    private void OnDisable()
    {
        Net.Instance.ConnectedToMaster -= this.HandleConnectedToMaster;
        Net.Instance.JoinedRoom -= this.HandleJoinedRoom;
        Net.Instance.Disconnected -= this.HandleDisconnected;
    }

    private void Start()
    {
        Debug.Log("NetAutoJoin: Connecting...");
        Net.Instance.ConnectUsingSettings();
    }

    private void HandleConnectedToMaster()
    {
        Debug.Log("NetAutoJoin: ConnectedToMaster -> Joining/Creating room...");

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        Net.Instance.JoinOrCreateRoom(
            roomName: this.roomName,
            roomOptions: options,
            typedLobby: TypedLobby.Default,
            expectedUsers: null
        );
    }

    private void HandleJoinedRoom()
    {
        Debug.Log("NetAutoJoin: JoinedRoom -> InRoom = " + Net.Instance.InRoom + ", Players = " + Net.Instance.CurrentRoom.PlayerCount);
    }

    private void HandleDisconnected()
    {
        Debug.Log("NetAutoJoin: Disconnected.");
    }
}
