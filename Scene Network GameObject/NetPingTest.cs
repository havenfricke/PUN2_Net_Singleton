using System;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

[RequireComponent(typeof(NetAutoJoin))]
public sealed class NetPingTest : MonoBehaviour
{
    private const byte EventCodePing = 1;

    private PhotonView photonView;

    private void Awake()
    {
        this.photonView = this.GetComponent<PhotonView>();
        if (this.photonView == null)
        {
            throw new InvalidOperationException("NetPingTest requires a PhotonView on the same GameObject.");
        }
    }

    private void OnEnable()
    {
        Net.Instance.JoinedRoom += this.HandleJoinedRoom;
        Net.Instance.RegisterEventHandler(EventCodePing, this.HandlePingEvent);
    }

    private void OnDisable()
    {
        if (Net.Instance != null)
        {
            Net.Instance.JoinedRoom -= this.HandleJoinedRoom;
            Net.Instance.UnregisterEventHandler(EventCodePing, this.HandlePingEvent);
        }
    }

    private void HandleJoinedRoom()
    {
        Debug.Log("NetPingTest: Ready. Press:");
        Debug.Log("  R = RPC broadcast");
        Debug.Log("  E = RaiseEvent broadcast");
    }

    private void Update()
    {
        if (Net.Instance.InRoom == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Net.Instance.Rpc(
                view: this.photonView,
                methodName: "Rpc_LogMessage",
                target: RpcTarget.All,
                parameters: new object[] { "RPC says hi from " + Net.Instance.LocalPlayer.NickName }
            );
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            string message = "EVENT says hi from " + Net.Instance.LocalPlayer.NickName;

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.All;

            SendOptions sendOptions = SendOptions.SendReliable;

            Net.Instance.RaiseEvent(
                eventCode: EventCodePing,
                eventContent: message,
                raiseEventOptions: options,
                sendOptions: sendOptions
            );
        }
    }

    [PunRPC]
    private void Rpc_LogMessage(string message)
    {
        Debug.Log("NetPingTest: " + message);
    }

    private void HandlePingEvent(EventData eventData)
    {
        object content = eventData.CustomData;
        Debug.Log("NetPingTest: " + (content == null ? "(null)" : content.ToString()));
    }
}
