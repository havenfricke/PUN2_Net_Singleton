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
        
        if (photonView == null)
        {
            photonView = GetComponent<PhotonView>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void SetTrigger(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            throw new ArgumentException("triggerName must be non-empty.", "triggerName");
        }

        animator.SetTrigger(triggerName);

        if (photonView.IsMine == false)
        {
            return;
        }

        Net.Instance.Rpc(
            view: photonView,
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

        animator.SetBool(boolName, value);

        if (photonView.IsMine == false)
        {
            return;
        }

        Net.Instance.Rpc(
            view: photonView,
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

        animator.SetFloat(floatName, value);

        if (photonView.IsMine == false)
        {
            return;
        }

        Net.Instance.Rpc(
            view: photonView,
            methodName: "Rpc_SetFloat",
            target: RpcTarget.Others,
            parameters: new object[] { floatName, value }
        );
    }

    public void CrossFade(string stateName, float transitionDuration, int layer = 0, float normalizedTime = 0f)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            throw new ArgumentException("stateName must be non-empty.", nameof(stateName));
        }

        animator.CrossFade(stateName, transitionDuration, layer, normalizedTime);

        if (!photonView.IsMine)
            return;

        Net.Instance.Rpc(
            view: photonView,
            methodName: "Rpc_CrossFade",
            target: RpcTarget.Others,
            parameters: new object[] { stateName, transitionDuration, layer, normalizedTime }
        );
    }

    [PunRPC]
    private void Rpc_SetTrigger(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            return;
        }

        animator.SetTrigger(triggerName);
    }

    [PunRPC]
    private void Rpc_SetBool(string boolName, bool value)
    {
        if (string.IsNullOrEmpty(boolName))
        {
            return;
        }

        animator.SetBool(boolName, value);
    }

    [PunRPC]
    private void Rpc_SetFloat(string floatName, float value)
    {
        if (string.IsNullOrEmpty(floatName))
        {
            return;
        }

        animator.SetFloat(floatName, value);
    }

    [PunRPC]
    private void Rpc_CrossFade(string stateName, float transitionDuration, int layer, float normalizedTime)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            return;
        }

        animator.CrossFade(stateName, transitionDuration, layer, normalizedTime);
    }
}
