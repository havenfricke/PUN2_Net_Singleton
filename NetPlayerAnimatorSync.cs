using System;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhotonView))]
public sealed class NetPlayerAnimatorSync : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private PhotonView photonView;

    private void Awake()
    {
        this.photonView = this.GetComponent<PhotonView>();
        if (this.photonView == null)
        {
            throw new InvalidOperationException("NetAnimatorSync requires a PhotonView on the same GameObject.");
        }

        if (this.animator == null)
        {
            this.animator = this.GetComponent<Animator>();
        }

        if (this.animator == null)
        {
            throw new InvalidOperationException("NetAnimatorSync requires an Animator (assign in inspector or place in children).");
        }
    }

    public void SetTrigger(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            throw new ArgumentException("triggerName must be non-empty.", "triggerName");
        }

        this.animator.SetTrigger(triggerName);

        if (this.photonView.IsMine == false)
        {
            return;
        }

        Net.Instance.Rpc(
            view: this.photonView,
            methodName: "Rpc_SetTrigger",
            target: RpcTarget.Others,
            parameters: new object[] { triggerName }
        );
    }

    public void SetBool(string boolName, bool value)
    {
        if (string.IsNullOrEmpty(boolName))
        {
            throw new ArgumentException("boolName must be non-empty.", "boolName");
        }

        this.animator.SetBool(boolName, value);

        if (this.photonView.IsMine == false)
        {
            return;
        }

        Net.Instance.Rpc(
            view: this.photonView,
            methodName: "Rpc_SetBool",
            target: RpcTarget.Others,
            parameters: new object[] { boolName, value }
        );
    }

    public void SetFloat(string floatName, float value)
    {
        if (string.IsNullOrEmpty(floatName))
        {
            throw new ArgumentException("floatName must be non-empty.", "floatName");
        }

        this.animator.SetFloat(floatName, value);

        if (this.photonView.IsMine == false)
        {
            return;
        }

        Net.Instance.Rpc(
            view: this.photonView,
            methodName: "Rpc_SetFloat",
            target: RpcTarget.Others,
            parameters: new object[] { floatName, value }
        );
    }

    [PunRPC]
    private void Rpc_SetTrigger(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            return;
        }

        this.animator.SetTrigger(triggerName);
    }

    [PunRPC]
    private void Rpc_SetBool(string boolName, bool value)
    {
        if (string.IsNullOrEmpty(boolName))
        {
            return;
        }

        this.animator.SetBool(boolName, value);
    }

    [PunRPC]
    private void Rpc_SetFloat(string floatName, float value)
    {
        if (string.IsNullOrEmpty(floatName))
        {
            return;
        }

        this.animator.SetFloat(floatName, value);
    }
}
