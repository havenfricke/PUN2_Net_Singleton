using UnityEngine;
using Photon.Pun;

public sealed class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

    private PhotonView photonView;

    private void Awake()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.currentHealth = this.maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int newHealth = this.currentHealth - amount;
        if (newHealth < 0)
        {
            newHealth = 0;
        }

        this.SetHealth(newHealth);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int newHealth = this.currentHealth + amount;
        if (newHealth > this.maxHealth)
        {
            newHealth = this.maxHealth;
        }

        this.SetHealth(newHealth);
    }

    public int GetHealth()
    {
        return this.currentHealth;
    }

    private void SetHealth(int value)
    {
        if (this.currentHealth == value)
        {
            return;
        }

        this.currentHealth = value;

        Net.Instance.Rpc(
            this.photonView,
            "Rpc_SetHealth",
            RpcTarget.All,
            new object[] { this.currentHealth }
        );
    }

    [PunRPC]
    private void Rpc_SetHealth(int syncedHealth)
    {
        this.currentHealth = syncedHealth;

        if (this.currentHealth <= 0)
        {
            this.OnDeath();
        }
    }

    private void OnDeath()
    {
        Debug.Log("Player died.");
    }
}
