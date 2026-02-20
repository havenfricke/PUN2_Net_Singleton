using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AvailableSpawnHandler : MonoBehaviour
{
    private PlayerSpawner playerSpawner;

    private void Awake()
    {
        if (playerSpawner == null)
            playerSpawner = GetComponentInParent<PlayerSpawner>();

        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Damageable"))
        {
            playerSpawner.SetSpawnAvailability(transform, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Damageable"))
        {
            playerSpawner.SetSpawnAvailability(transform, true);
        }
    }
}