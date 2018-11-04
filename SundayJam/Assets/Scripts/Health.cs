using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{

    public const int maxHealth = 100;
    public const int baseTankiness = 0;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    public void TakeDamage(int amount, PlayerController owner)
    {
        if (!isServer)
            return;

        currentHealth -= (amount - baseTankiness);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            owner.Kills++;
            GetComponent<PlayerController>().Deaths++;
            RpcRespawn();
        }
    }

    public void AddHealth(int amount)
    {
        if (!isServer)
            return;
        currentHealth += amount;
    }

    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            var spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick a spawn point at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
            currentHealth = maxHealth;
        }
    }
}