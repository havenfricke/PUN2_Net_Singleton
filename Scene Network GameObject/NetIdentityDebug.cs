using Photon.Pun;
using UnityEngine;

public sealed class NetDebugIdentity : MonoBehaviour
{
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] I see Player obj name={gameObject.name} " +
                  $"ViewID={(pv ? pv.ViewID : -1)} IsMine={(pv ? pv.IsMine : false)} " +
                  $"Owner={(pv && pv.Owner != null ? pv.Owner.ActorNumber : -1)} " +
                  $"Pos={transform.position}");
    }
}
