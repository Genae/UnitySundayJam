using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{

    public const int maxHealth = 100;
    public const int baseTankiness = 0;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthBar;
    public AudioClip[] hitSounds;
    public AudioClip[] hurtSounds;

    public void TakeDamage(int amount, PlayerController owner)
    {
        if (!isServer)
            return;

        currentHealth -= (amount - baseTankiness);
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
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
        currentHealth = health;
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
        AudioSource.PlayClipAtPoint(hitSounds[Random.Range(0, hitSounds.Length)], this.transform.position);
        AudioSource.PlayClipAtPoint(hurtSounds[Random.Range(0, hurtSounds.Length)], this.transform.position);
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
            OnChangeHealth(maxHealth);
        }
    }
}